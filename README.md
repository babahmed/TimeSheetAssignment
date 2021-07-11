# TimeSheetAssignment
Himama Assignment Report

I used a .net core 5 and CQRS which separates all layers of application
More work but with better code maintainability.

I used a mediator framework for the implementation of the calls, this can be reusable if an API is to be layered on this application.An extension of .net core Entity layer with auto logging which logs all user actions for objects derived from auditable entities.

Implementation approach was TDD design; most test cases were written based on the expected logic implementation and requirement of the application.

In memory db was used for testing as migration could not be added until the web project is set as startup for DbContext injection. This is because there was no need for an API project.
This caused quite a bit of challenge and more time was spent setting up the test dependencies.

Initial plan was to make Timesheet entity with timeout nullable DateTime and Timein Defaulted as now i however had to modify the TimeOut to non-nullable due to the restriction around the DateTimePicker in blazor UI.

Used Admin LTE template bootstrap for ease of ui development. I needed to speed up the ui as I already spent too much time on the backend structure. As earlier stated, CQRS requires more code.

Use different context and migration to manage Identity and application context, this is to separate the auditable entities repository layer from the identity entities.

Added migrations into separate folders.

Added google login for ease of authentication

If There was more time I would have:

Introduced redis for cached data calls. The GetOpenTrackerQuery would be the best candidate.

Completed the empty tests and added a lot more tests to cater for the conditions in the handlers. The test setup is complete and adding tests would have been a lot easier.

Check if a list of Timesheet exists before rendering instead of rendering empty tables on the get all tracker UI.

Add SonarQube analysis for Code quality and fix all spotted vulnerabilities like the google client code uploaded to GIT from appsettings development.

Add Controller and UI tests.

