UPDATE Employees SET FirstName = @firstName,
LastName = @lastName,
HireDate = @hireDate,
FavoriteColor = @favoriteColor
WHERE EmployeeId = @employeeId;