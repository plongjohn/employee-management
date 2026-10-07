<?php

declare(strict_types=1);

namespace EmployeeManagement;

use EmployeeManagement\Config\ConfigurationException;
use EmployeeManagement\Http\CsrfTokenManager;
use EmployeeManagement\Http\Request;
use EmployeeManagement\Http\Response;
use EmployeeManagement\View\ErrorPageRenderer;
use LogicException;
use PDOException;
use Psr\Container\ContainerInterface;
use Psr\Log\LoggerInterface;
use Throwable;

/**
 * Turns a request into a response: finds the route, checks the CSRF token of form posts,
 * calls the controller and converts errors into friendly error pages.
 */
final readonly class Application
{
    private const array SECURITY_HEADERS = [
        'X-Content-Type-Options' => 'nosniff',
        'X-Frame-Options' => 'DENY',
        'Referrer-Policy' => 'same-origin',
    ];

    public function __construct(
        private Router $router,
        private ContainerInterface $container,
        private CsrfTokenManager $csrfTokenManager,
        private ErrorPageRenderer $errorPages,
        private LoggerInterface $logger,
    ) {
    }

    public function handle(Request $request): Response
    {
        $response = $this->dispatch($request);

        return new Response($response->statusCode, $response->body, $response->headers + self::SECURITY_HEADERS);
    }

    private function dispatch(Request $request): Response
    {
        try {
            $match = $this->router->match($request->method, $request->path);
            if ($match === null) {
                return $this->errorPages->render(404, 'ErrorNotFoundHeading', 'ErrorNotFound');
            }

            if ($request->isPost() && !$this->csrfTokenManager->isValid($request->formValue(CsrfTokenManager::FIELD_NAME))) {
                $this->logger->warning('Rejected form post to {path}: invalid CSRF token', ['path' => $request->path]);

                return $this->errorPages->render(400, 'ErrorHeading', 'ErrorInvalidRequest');
            }

            return $this->callController($match, $request->withRouteParameters($match->parameters));
        } catch (ConfigurationException $exception) {
            $this->logger->critical($exception->getMessage(), ['exception' => $exception]);

            return $this->errorPages->render(503, 'ErrorHeading', 'ErrorConfigurationInvalid', $exception);
        } catch (PDOException $exception) {
            $this->logger->error('Database access failed', ['exception' => $exception]);

            return $this->errorPages->render(503, 'ErrorHeading', 'ErrorDatabaseUnavailable', $exception);
        } catch (Throwable $exception) {
            $this->logger->error('Unexpected error while handling {path}', [
                'path' => $request->path,
                'exception' => $exception,
            ]);

            return $this->errorPages->render(500, 'ErrorHeading', 'ErrorUnexpected', $exception);
        }
    }

    private function callController(RouteMatch $match, Request $request): Response
    {
        [$controllerClass, $method] = $match->handler;
        $action = [$this->container->get($controllerClass), $method];
        if (!is_callable($action)) {
            throw new LogicException("Route handler {$controllerClass}::{$method} is not callable.");
        }

        $response = $action($request);
        if (!$response instanceof Response) {
            throw new LogicException("Route handler {$controllerClass}::{$method} did not return a response.");
        }

        return $response;
    }
}
