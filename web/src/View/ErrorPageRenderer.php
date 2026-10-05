<?php

declare(strict_types=1);

namespace EmployeeManagement\View;

use EmployeeManagement\Config\AppConfig;
use EmployeeManagement\Http\Response;
use Psr\Log\LoggerInterface;
use Throwable;
use Twig\Environment;

final readonly class ErrorPageRenderer
{
    public function __construct(
        private Environment $twig,
        private Translator $translator,
        private AppConfig $config,
        private LoggerInterface $logger,
    ) {
    }

    public function render(int $statusCode, string $headingKey, string $messageKey, ?Throwable $error = null): Response
    {
        $heading = $this->translator->translate($headingKey);
        $message = $this->translator->translate($messageKey);
        $details = $error !== null && $this->config->debug() ? (string) $error : null;

        try {
            return Response::html(
                $this->twig->render('errors/error.html.twig', [
                    'heading' => $heading,
                    'message' => $message,
                    'details' => $details,
                ]),
                $statusCode,
            );
        } catch (Throwable $renderError) {
            // The error page must not fail as well, e.g. when a template is broken.
            $this->logger->error('Error page could not be rendered', ['exception' => $renderError]);

            return Response::html(
                '<!doctype html><meta charset="utf-8"><h1>' . htmlspecialchars($heading) . '</h1><p>'
                    . htmlspecialchars($message) . '</p>',
                $statusCode,
            );
        }
    }
}
