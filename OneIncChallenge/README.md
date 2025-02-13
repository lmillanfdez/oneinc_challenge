## Best practices
- Implementing controllers responsibles for validating the data sent in requests, handling different types of exceptions and logging them accordingly.
- Implementing services which manipulates both the incoming data before sending it to the data store and the data to be sent as the request's response coming from the data store.  
- Implementing the logic for interacting with the data store inside repositories allows modifying it without making changes the components getting or sending data from or to the data store.
- Creating the "Users" table under the "Challenge" schema inside the DB in order to follow a pattern in which tables belongs to a specific domain.
- Implementing custom exceptions in order to better identify the issue to handle and thus being able to log it and return a response accordingly.  
- Using dependency injection in order to decouple the creation of classes from the creation of its dependencies, which allows to modify the approach for initializing a dependency without modifying the initialization of the class consuming it.
- The implemented Http PUT and DELETE methods provides idempotency, which increases the reliability of the application.
- According to [definition](https://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html) of the Http PUT method, an user is created if it does not exist previously.

## Logging
Logs are stored inside a "Logs" folder in the root directory of this project.

## DB scripts

The script for creating the "Users" table can be found inside the "SQLScripts" folder. Please note this table is created under the "Challenge" schema. If you do not pretend to follow this desing, make changes in both this script and the queries defined inside the "UsersRepository" file.