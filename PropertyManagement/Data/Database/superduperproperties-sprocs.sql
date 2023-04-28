/*
USE ReallyGoodPropertyManagement;  
GO  
CREATE PROCEDURE RetrieveAllProperties   
AS
SELECT * FROM Property AS p
	INNER JOIN Owner AS o ON o.OwnerID = p.OwnerId
	LEFT JOIN Lease AS l ON l.PropertyId = p.PropertyID
	LEFT JOIN Renter AS r ON r.LeaseId = l.LeaseID;
GO
*/

/*
USE ReallyGoodPropertyManagement;  
GO  
CREATE PROCEDURE GetAllProperties   
AS
SELECT o.OwnerID, o.FirstName AS Owner_FirstName, o.LastName AS Owner_LastName, o.Email AS Owner_Email, o.Phone AS Owner_Phone, -- Alias to avoid conflict with Renter columns
	p.PropertyID, p.Property_OwnerId, AddressLine1, AddressLine2, City, StateCode, ZipCode, Bedrooms, Bathrooms, PetsAllowed, UnitNumber, WasherDryer, Dishwasher,
	l.LeaseID, l.Lease_PropertyId, StartDate, EndDate, MonthlyRent, RentOutstanding, SecurityDepositAmount, SecurityDepositPaid, SecurityDepositCharges, SecurityDepositReturned,
	RenterID, r.Renter_LeaseId, r.FirstName, r.LastName, r.Email, r.Phone
FROM Owner AS o
	INNER JOIN Property AS p ON p.Property_OwnerId = o.OwnerID
	LEFT JOIN Lease AS l ON l.Lease_PropertyId = p.PropertyID
	LEFT JOIN Renter AS r ON r.Renter_LeaseId = l.LeaseID
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE GetRenter
	@RenterId int
AS
SELECT * FROM Renter WHERE RenterID = @RenterId
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE GetLease
	@LeaseId int
AS
SELECT * FROM Lease WHERE LeaseID = @LeaseId
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE GetProperty
	@PropertyId int
AS
SELECT * FROM Property WHERE PropertyID = @PropertyId
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE GetOwner
	@OwnerId int
AS
SELECT * FROM Owner WHERE OwnerID = @OwnerId
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE UpdateRenter
	@RenterId int,
	@FirstName varchar(50),
	@LastName varchar(50),
	@Email varchar(50),
	@Phone varchar(50)
AS
UPDATE Renter
	SET FirstName = @FirstName,
		LastName = @LastName,
		Email = @Email,
		Phone = @Phone
	WHERE RenterID = @RenterId
		-- Don't update if no values have changed
		AND (FirstName <> @firstName OR LastName <> @lastName OR Email <> @Email OR Phone <> @Phone);
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE UpdateLease
	@LeaseID int,
	@StartDate datetime,
	@EndDate datetime,
	@MonthlyRent decimal(15,2),
	@RentOutstanding decimal(15,2),
	@SecurityDepositAmount decimal(15,2),
	@SecurityDepositPaid bit,
	@SecurityDepositCharges decimal(15,2),
	@SecurityDepositReturned bit
AS
UPDATE Lease
SET
	StartDate = @StartDate,
	EndDate = @EndDate,
	MonthlyRent = @MonthlyRent,
	RentOutstanding = @RentOutstanding,
	SecurityDepositAmount = @SecurityDepositAmount,
	SecurityDepositPaid = @SecurityDepositPaid,
	SecurityDepositCharges = @SecurityDepositCharges,
	SecurityDepositReturned = @SecurityDepositReturned
WHERE LeaseID = @LeaseId
	AND (StartDate <> @StartDate OR EndDate <> @EndDate OR MonthlyRent <> @MonthlyRent
		-- Simply checking <> on nullable columns doesn't work
		OR (@RentOutstanding IS NOT NULL AND RentOutstanding IS NULL) OR (@RentOutstanding IS NULL AND RentOutstanding IS NOT NULL) OR RentOutstanding <> @RentOutstanding
		OR (@SecurityDepositAmount IS NOT NULL AND SecurityDepositAmount IS NULL) OR (@SecurityDepositAmount IS NULL AND SecurityDepositAmount IS NOT NULL) OR SecurityDepositAmount <> @SecurityDepositAmount
		OR (@SecurityDepositPaid IS NOT NULL AND SecurityDepositPaid IS NULL) OR (@SecurityDepositPaid IS NULL AND SecurityDepositPaid IS NOT NULL) OR SecurityDepositPaid <> @SecurityDepositPaid
		OR (@SecurityDepositCharges IS NOT NULL AND SecurityDepositCharges IS NULL) OR (@SecurityDepositCharges IS NULL AND SecurityDepositCharges IS NOT NULL) OR SecurityDepositCharges <> @SecurityDepositCharges
		OR (@SecurityDepositReturned IS NOT NULL AND SecurityDepositReturned IS NULL) OR (@SecurityDepositReturned IS NULL AND SecurityDepositReturned IS NOT NULL) OR SecurityDepositReturned <> @SecurityDepositReturned)
;
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE UpdateProperty
	@PropertyID int,
	@AddressLine1 varchar(100),
	@AddressLine2 varchar(100),
	@City varchar(50),
	@StateCode varchar(2),
	@ZipCode varchar(20),
	@Bedrooms int,
	@Bathrooms int,
	@PetsAllowed bit,
	@UnitNumber varchar(20),
	@WasherDryer bit,
	@Dishwasher bit
AS
UPDATE Property
SET
	AddressLine1 = @AddressLine1,
	AddressLine2 = @AddressLine2,
	City = @City,
	StateCode = @StateCode,
	ZipCode = @ZipCode,
	Bedrooms = @Bedrooms,
	Bathrooms = @Bathrooms,
	PetsAllowed = @PetsAllowed,
	UnitNumber = @UnitNumber,
	WasherDryer = @WasherDryer,
	Dishwasher = @Dishwasher
WHERE PropertyID = @PropertyID
	AND (
		AddressLine1 <> @AddressLine1
		OR (@AddressLine2 IS NOT NULL AND AddressLine2 IS NULL) OR (@AddressLine2 IS NULL AND AddressLine2 IS NOT NULL) OR AddressLine2 <> @AddressLine2
		OR City <> @City
		OR StateCode <> @StateCode
		OR ZipCode <> @ZipCode
		OR Bedrooms <> @Bedrooms
		OR Bathrooms <> @Bathrooms
		OR (@PetsAllowed IS NOT NULL AND PetsAllowed IS NULL) OR (@PetsAllowed IS NULL AND PetsAllowed IS NOT NULL) OR PetsAllowed <> @PetsAllowed
		OR (@UnitNumber IS NOT NULL AND UnitNumber IS NULL) OR (@UnitNumber IS NULL AND UnitNumber IS NOT NULL) OR UnitNumber <> @UnitNumber
		OR (@WasherDryer IS NOT NULL AND WasherDryer IS NULL) OR (@WasherDryer IS NULL AND WasherDryer IS NOT NULL) OR WasherDryer <> @WasherDryer
		OR (@Dishwasher IS NOT NULL AND Dishwasher IS NULL) OR (@Dishwasher IS NULL AND Dishwasher IS NOT NULL) OR Dishwasher <> @Dishwasher
	);
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE UpdateOwner
	@OwnerId int,
	@FirstName varchar(50),
	@LastName varchar(50),
	@Email varchar(50),
	@Phone varchar(50)
AS
UPDATE Owner
	SET FirstName = @FirstName,
		LastName = @LastName,
		Email = @Email,
		Phone = @Phone
	WHERE OwnerID = @OwnerId
		-- Don't update if no values have changed
		AND (FirstName <> @FirstName OR LastName <> @LastName OR Email <> @Email OR Phone <> @Phone);
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE InsertProperty
	@Property_OwnerID int,
	@AddressLine1 varchar(100),
	@AddressLine2 varchar(100),
	@City varchar(50),
	@StateCode varchar(2),
	@ZipCode varchar(20),
	@Bedrooms int,
	@Bathrooms int,
	@PetsAllowed bit,
	@UnitNumber varchar(20),
	@WasherDryer bit,
	@Dishwasher bit
AS
	INSERT INTO Property
	(Property_OwnerID, AddressLine1, AddressLine2, City, StateCode, ZipCode, Bedrooms, Bathrooms, PetsAllowed, UnitNumber, WasherDryer, Dishwasher)
	VALUES (@Property_OwnerID, @AddressLine1, @AddressLine2, @City, @StateCode, @ZipCode, @Bedrooms, @Bathrooms, @PetsAllowed, @UnitNumber, @WasherDryer, @Dishwasher);
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE GetOwnerIdByEmail
	@Email varchar(50)
AS
	SELECT OwnerID FROM Owner
	WHERE Email = @Email;
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE InsertOwner
	@FirstName varchar(50),
	@LastName varchar(50),
	@Email varchar(50),
	@Phone varchar(50)
AS
	INSERT INTO Owner
	(FirstName, LastName, Email, Phone)
	VALUES (@FirstName, @LastName, @Email, @Phone);
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE InsertLease
	@Lease_PropertyID int,
	@StartDate datetime,
	@EndDate datetime,
	@MonthlyRent decimal(15,2),
	@RentOutstanding decimal(15,2),
	@SecurityDepositAmount decimal(15,2),
	@SecurityDepositPaid bit,
	@SecurityDepositCharges decimal(15,2),
	@SecurityDepositReturned bit
AS
	INSERT INTO Lease
	(Lease_PropertyId, StartDate, EndDate, MonthlyRent, RentOutstanding, SecurityDepositAmount, SecurityDepositPaid, SecurityDepositCharges, SecurityDepositReturned)
	VALUES (@Lease_PropertyID, @StartDate, @EndDate, @MonthlyRent, @RentOutstanding, @SecurityDepositAmount, @SecurityDepositPaid, @SecurityDepositCharges, @SecurityDepositReturned);
GO
*/
/*
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE InsertRenter
	@Renter_LeaseID int,
	@FirstName varchar(50),
	@LastName varchar(50),
	@Email varchar(50),
	@Phone varchar(50)
AS
	INSERT INTO Renter
	(Renter_LeaseId, FirstName, LastName, Email, Phone)
	VALUES (@Renter_LeaseID, @FirstName, @LastName, @Email, @Phone);
GO
*/
/*
-- Search and filter on property attributes
USE ReallyGoodPropertyManagement;
GO
CREATE PROCEDURE SearchProperties
	@City varchar(50), @StateCode varchar(2), @ZipCode varchar(20), @Bedrooms int, @Bathrooms int, @PetsAllowed bit, @WasherDryer bit, @Dishwasher bit
AS
SELECT *
	FROM Property
	WHERE (@City IS NULL OR City = @City)
	AND (@StateCode IS NULL OR StateCode = @StateCode)
	AND (@ZipCode IS NULL OR ZipCode = @ZipCode)
	AND (@Bedrooms IS NULL OR Bedrooms = @Bedrooms)
	AND (@Bathrooms IS NULL OR Bathrooms = @Bathrooms)
	AND (@PetsAllowed IS NULL OR PetsAllowed = @PetsAllowed)
	AND (@WasherDryer IS NULL OR WasherDryer = @WasherDryer)
	AND (@Dishwasher IS NULL OR Dishwasher = @Dishwasher)
GO
*/

