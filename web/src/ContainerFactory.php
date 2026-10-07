<?php

declare(strict_types=1);

namespace EmployeeManagement;

use DI\ContainerBuilder;
use EmployeeManagement\Config\AppConfig;
use EmployeeManagement\Config\Database;
use EmployeeManagement\Http\NativeSessionStorage;
use EmployeeManagement\Http\SessionStorage;
use EmployeeManagement\Repositories\DepartmentRepository;
use EmployeeManagement\Repositories\DepartmentRepositoryInterface;
use EmployeeManagement\Repositories\EmployeeRepository;
use EmployeeManagement\Repositories\EmployeeRepositoryInterface;
use EmployeeManagement\Services\SystemClock;
use EmployeeManagement\View\AppExtension;
use EmployeeManagement\View\Translator;
use Monolog\Handler\RotatingFileHandler;
use Monolog\Logger;
use Monolog\Processor\PsrLogMessageProcessor;
use PDO;
use Psr\Clock\ClockInterface;
use Psr\Container\ContainerInterface;
use Psr\Log\LoggerInterface;
use Twig\Environment;
use Twig\Loader\FilesystemLoader;

use function DI\autowire;
use function DI\factory;

/**
 * Wires up the application. Classes with unambiguous constructor dependencies are created by
 * autowiring; only interfaces and objects that need configuration are defined here.
 */
final class ContainerFactory
{
    // One log file per day; two weeks are enough to look into a reported problem.
    private const int LOG_FILES_KEPT = 14;

    public static function create(string $rootDirectory): ContainerInterface
    {
        $builder = new ContainerBuilder();
        $builder->useAutowiring(true);
        $builder->addDefinitions([
            AppConfig::class => factory(static fn (): AppConfig => AppConfig::fromFile("{$rootDirectory}/config/config.php")),

            // Connects on first use, so pages without database access still work when it is down.
            PDO::class => factory(static fn (AppConfig $config): PDO => Database::connect($config->database())),

            LoggerInterface::class => factory(static fn (AppConfig $config): LoggerInterface => new Logger(
                'web',
                [new RotatingFileHandler(
                    "{$rootDirectory}/var/log/employee-management.log",
                    self::LOG_FILES_KEPT,
                    $config->logLevel(),
                )],
                [new PsrLogMessageProcessor(removeUsedContextFields: true)],
            )),

            Translator::class => factory(static fn (): Translator => Translator::fromFile("{$rootDirectory}/lang/de.php")),

            Environment::class => factory(static function (AppConfig $config, AppExtension $extension) use ($rootDirectory): Environment {
                $twig = new Environment(new FilesystemLoader("{$rootDirectory}/templates"), [
                    'cache' => "{$rootDirectory}/var/cache/twig",
                    // Recompiles a template when its file changes, so edits show up without clearing the cache.
                    'auto_reload' => true,
                    'strict_variables' => $config->debug(),
                ]);
                $twig->addExtension($extension);

                return $twig;
            }),

            Router::class => factory(static function () use ($rootDirectory): Router {
                $router = new Router();
                (require "{$rootDirectory}/config/routes.php")($router);

                return $router;
            }),

            ClockInterface::class => autowire(SystemClock::class),
            SessionStorage::class => autowire(NativeSessionStorage::class),
            EmployeeRepositoryInterface::class => autowire(EmployeeRepository::class),
            DepartmentRepositoryInterface::class => autowire(DepartmentRepository::class),
        ]);

        return $builder->build();
    }
}
