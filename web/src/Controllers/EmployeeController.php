<?php

declare(strict_types=1);

namespace EmployeeManagement\Controllers;

use EmployeeManagement\Http\FlashMessages;
use EmployeeManagement\Http\FlashType;
use EmployeeManagement\Http\Request;
use EmployeeManagement\Http\Response;
use EmployeeManagement\Models\Employee;
use EmployeeManagement\Models\EmployeeInput;
use EmployeeManagement\Models\EmployeeQuery;
use EmployeeManagement\Services\DepartmentService;
use EmployeeManagement\Services\EmployeeService;
use EmployeeManagement\Services\EmployeeValidator;
use EmployeeManagement\Services\OperationResult;
use EmployeeManagement\Services\OperationStatus;
use EmployeeManagement\View\AppExtension;
use EmployeeManagement\View\Translator;
use EmployeeManagement\View\ValidationMessages;
use LogicException;
use Psr\Clock\ClockInterface;
use Twig\Environment;

/**
 * Every form carries the list state (search, filter, sorting, page) in hidden fields, so the
 * user returns to the same list view after saving, cancelling or deleting.
 */
final readonly class EmployeeController
{
    private const int UNPROCESSABLE_CONTENT = 422;
    private const int CONFLICT = 409;

    public function __construct(
        private EmployeeService $employeeService,
        private EmployeeValidator $validator,
        private DepartmentService $departmentService,
        private ValidationMessages $validationMessages,
        private Translator $translator,
        private FlashMessages $flashMessages,
        private ClockInterface $clock,
        private Environment $twig,
    ) {
    }

    public function home(Request $request): Response
    {
        return Response::redirect(AppExtension::listUrl(new EmployeeQuery()));
    }

    public function index(Request $request): Response
    {
        $query = EmployeeQuery::fromParameters($request->query);
        $result = $this->employeeService->search($query);

        if ($result->isBeyondLastPage()) {
            return Response::redirect(AppExtension::listUrl($query->withPage($result->totalPages())));
        }

        return $this->render('employees/index.html.twig', [
            'query' => $query,
            'result' => $result,
            'departments' => $this->departmentService->findAll(),
            'pageSizeOptions' => EmployeeQuery::PAGE_SIZE_OPTIONS,
        ]);
    }

    /**
     * Only the table with its footer, for searching while typing and the background refresh.
     * A page beyond the last one shows the last page instead of redirecting: the fragment
     * carries its own list URL, which the script puts into the address bar.
     */
    public function listFragment(Request $request): Response
    {
        $query = EmployeeQuery::fromParameters($request->query);
        $result = $this->employeeService->search($query);

        if ($result->isBeyondLastPage()) {
            $query = $query->withPage($result->totalPages());
            $result = $this->employeeService->search($query);
        }

        return $this->render('employees/_list.html.twig', [
            'query' => $query,
            'result' => $result,
            'pageSizeOptions' => EmployeeQuery::PAGE_SIZE_OPTIONS,
        ]);
    }

    public function create(Request $request): Response
    {
        $input = EmployeeInput::empty($this->clock->now());

        return $this->renderForm(EmployeeQuery::fromParameters($request->query), $input, $this->encodeOriginal($input));
    }

    public function store(Request $request): Response
    {
        $listQuery = EmployeeQuery::fromParameters($request->form);
        $result = $this->employeeService->create(EmployeeInput::fromForm($request->form));

        if ($result->isSuccess()) {
            return $this->redirectToList($listQuery, FlashType::Success, 'StatusCreated');
        }

        return $this->renderFailedForm($request, $listQuery, $result);
    }

    public function edit(Request $request): Response
    {
        $listQuery = EmployeeQuery::fromParameters($request->query);
        $employee = $this->employeeService->findById($request->routeInt('id'));
        if ($employee === null) {
            return $this->redirectToList($listQuery, FlashType::Warning, 'EmployeeNotFound');
        }

        $input = $employee->toInput();

        return $this->renderForm($listQuery, $input, $this->encodeOriginal($input), $employee->id, $employee->version);
    }

    public function update(Request $request): Response
    {
        $listQuery = EmployeeQuery::fromParameters($request->form);
        $id = $request->routeInt('id');
        $result = $this->employeeService->update(
            $id,
            $request->formInt('version') ?? 0,
            EmployeeInput::fromForm($request->form),
        );

        return match ($result->status) {
            OperationStatus::Success => $this->redirectToList($listQuery, FlashType::Success, 'StatusSaved'),
            OperationStatus::NotFound => $this->redirectToList($listQuery, FlashType::Warning, 'EmployeeNotFound'),
            default => $this->renderFailedForm($request, $listQuery, $result, $id),
        };
    }

    public function delete(Request $request): Response
    {
        $listQuery = EmployeeQuery::fromParameters($request->form);
        $result = $this->employeeService->delete($request->routeInt('id'), $request->formInt('version') ?? 0);

        // An emptied last page is corrected by index(), which redirects to the new last page.
        return match ($result->status) {
            OperationStatus::Success => $this->redirectToList($listQuery, FlashType::Success, 'StatusDeleted'),
            OperationStatus::Conflict => $this->redirectToList($listQuery, FlashType::Warning, 'DeleteConflict'),
            OperationStatus::NotFound => $this->redirectToList($listQuery, FlashType::Warning, 'DeleteNotFound'),
            default => throw new LogicException("Unexpected delete result {$result->status->name}."),
        };
    }

    /**
     * Shows the form again with the user's input and what went wrong. The original values
     * travel along, so the save button still knows whether anything was changed.
     *
     * @param OperationResult<Employee> $result
     */
    private function renderFailedForm(
        Request $request,
        EmployeeQuery $listQuery,
        OperationResult $result,
        ?int $employeeId = null,
    ): Response {
        $fieldErrors = $result->status === OperationStatus::DuplicateEmail
            ? ['email' => $this->translator->translate('DuplicateEmail')]
            : $this->validationMessages->byField($result->errors);
        $isConflict = $result->status === OperationStatus::Conflict;

        return $this->renderForm(
            $listQuery,
            EmployeeInput::fromForm($request->form),
            // Only used by the browser to enable the save button, so it is passed on as it came.
            $request->formValue('original'),
            $employeeId,
            $request->formInt('version'),
            $fieldErrors,
            $isConflict,
            $isConflict ? self::CONFLICT : self::UNPROCESSABLE_CONTENT,
        );
    }

    /**
     * @param array<string, string> $fieldErrors
     */
    private function renderForm(
        EmployeeQuery $listQuery,
        EmployeeInput $input,
        string $originalJson,
        ?int $employeeId = null,
        ?int $version = null,
        array $fieldErrors = [],
        bool $isConflict = false,
        int $statusCode = 200,
    ): Response {
        return $this->render('employees/form.html.twig', [
            'listQuery' => $listQuery,
            'employeeId' => $employeeId,
            'version' => $version,
            'values' => $input->toForm(),
            'originalJson' => $originalJson,
            'fieldErrors' => $fieldErrors,
            'isConflict' => $isConflict,
            'departments' => $this->departmentService->findAll(),
            'nameMaxLength' => EmployeeValidator::NAME_MAX_LENGTH,
            'emailMaxLength' => EmployeeValidator::EMAIL_MAX_LENGTH,
            'earliestHireDate' => EmployeeValidator::earliestHireDate()->format(EmployeeInput::DATE_FORMAT),
            'latestHireDate' => $this->validator->latestHireDate()->format(EmployeeInput::DATE_FORMAT),
        ], $statusCode);
    }

    private function encodeOriginal(EmployeeInput $original): string
    {
        return json_encode($original->toForm(), JSON_THROW_ON_ERROR);
    }

    private function redirectToList(EmployeeQuery $listQuery, FlashType $type, string $textKey): Response
    {
        $this->flashMessages->add($type, $textKey);

        return Response::redirect(AppExtension::listUrl($listQuery));
    }

    /**
     * @param array<string, mixed> $context
     */
    private function render(string $template, array $context, int $statusCode = 200): Response
    {
        return Response::html($this->twig->render($template, $context), $statusCode);
    }
}
