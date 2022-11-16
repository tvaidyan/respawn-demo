  INSERT INTO Employees (EmployeeId, FirstName, LastName, HireDate, FavoriteColor)
	  VALUES(NEWID(), @firstName, @lastName, @hireDate, @favoriteColor);