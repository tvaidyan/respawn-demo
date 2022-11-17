DECLARE @employeeId UNIQUEIDENTIFIER;
SET @employeeId = NEWID();

INSERT INTO Employees (EmployeeId, FirstName, LastName, HireDate, FavoriteColor)
	VALUES(@employeeId, @firstName, @lastName, @hireDate, @favoriteColor);

SELECT TOP 1 EmployeeId, FirstName, LastName, HireDate, FavoriteColor
FROM Employees 
WHERE EmployeeId = @employeeId;