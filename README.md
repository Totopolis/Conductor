# Conductor

Reference .NET project in the process orchestration domain area.

## Covered topics

1. Clean architecture solution structure (onion)

2. Domain driven design (DDD): Entity, ValueObject, Smart enum, Aggregate, Domain event

3. Event based app: MassTransit

4. Tests (AAA): domain, app, infrastructure (TestContainers, NEED DOCKER DESKTOP!), archi

5. Configuration, consul, options, options validators, observabled options, each layer has itself options

6. Open telemetry: logs, traces and metrics

7. REPR-pattern: FastEndpoints without HTTPS (presentation layer)

8. NodaTime use

9. Syntax analyzers: cutted codestyle, banned list

10. EF Core with DDD: repository, unit of work, migrations

11. Domain events: transactional outbox pattern

12. A little bit of "AI"

13. Docker & CI & Versioning

14. Healchecks and readness (k8s environment)

15. Result pattern: factory and other methods, combine errors, type-code-description, metadata

16. Api: request validate and errors handling

17. CQRS: commands and queries through MediatR

18. Secret manager (setup Consul section like in appsettings.development.json): https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets

19. SemVer inside CI

## References

- .NET BIBLE https://www.youtube.com/@nickchapsas

- .NET BIBLE: https://www.youtube.com/@MilanJovanovicTech

- DDD thoughts: https://www.youtube.com/watch?v=kLLsVT_53bw&list=PL2E-vlKoo_v3ch9oZWYZWwRbqdVoWHY8X

- DDD Aggregate in EF: https://www.youtube.com/watch?v=5_un3PUER8U

- Vladimir Khorikov (TDD, DDD): https://enterprisecraftsmanship.com/posts

- Functional approach: https://github.com/vkhorikov/CSharpFunctionalExtensions

- ErrorOr (result pattern): https://github.com/amantinband/error-or

- HTTP API: https://fast-endpoints.com/

- OTEL (RUS): https://www.youtube.com/watch?v=X3faF3xw3m8

- Test containers: https://dotnet.testcontainers.org/ https://blog.jetbrains.com/dotnet/2023/10/24/how-to-use-testcontainers-with-dotnet-unit-tests/

## Need to consider

- https://andrewlock.net/preventing-breaking-changes-in-public-apis-with-publicapigenerator/

- https://www.youtube.com/watch?v=IsmyqNrfQQw
