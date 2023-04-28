/*
CREATE TABLE Owner (
	OwnerID INT PRIMARY KEY IDENTITY(1, 1),
	FirstName varchar(50) NOT NULL,
	LastName varchar(50) NOT NULL,
	Email varchar(50) NOT NULL,
	Phone varchar(50) NOT NULL,
);
*/
/*
CREATE TABLE Property (
	PropertyID INT PRIMARY KEY IDENTITY(1, 1),
	OwnerId INT NOT NULL,
	AddressLine1 varchar(100) NOT NULL,
	AddressLine2 varchar(100),
	City varchar(50) NOT NULL,
	StateCode varchar(2) NOT NULL,
	ZipCode varchar(20) NOT NULL,
	Bedrooms INT NOT NULL,
	Bathrooms INT NOT NULL,
	PetsAllowed BIT,
	UnitNumber varchar(20),
	WasherDryer BIT,
	Dishwasher BIT,
	CONSTRAINT FK_Property_Owner FOREIGN KEY (OwnerId) REFERENCES dbo.Owner(OwnerID)
	ON DELETE NO ACTION
	ON UPDATE NO ACTION
);
*/
--ALTER TABLE Owner ADD CONSTRAINT FK_Owner_Address FOREIGN KEY (AddressId) REFERENCES dbo.Address(AddressID);
--EXECUTE SP_RENAME @objname = 'Owner.OwnerAddress', @newname = 'AddressId', @objtype = 'COLUMN';
/*
CREATE TABLE Lease (
	LeaseID INT PRIMARY KEY IDENTITY(1, 1),
	PropertyId INT NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	MonthlyRent decimal(15, 2) NOT NULL,
	RentOutstanding decimal(15, 2),
	SecurityDepositAmount decimal(15, 2),
	SecurityDepositPaid BIT,
	SecurityDepositCharges decimal(15, 2),
	SecurityDepositReturned BIT
	CONSTRAINT FK_Lease_Property FOREIGN KEY (PropertyId) REFERENCES dbo.Property(PropertyID)
	ON DELETE NO ACTION
	ON UPDATE NO ACTION,
);
*/
/*
CREATE TABLE Renter (
	RenterID INT PRIMARY KEY IDENTITY(1, 1),
	LeaseId INT NOT NULL,
	FirstName varchar(50) NOT NULL,
	LastName varchar(50) NOT NULL,
	Email varchar(50) NOT NULL,
	Phone varchar(50) NOT NULL,
	CONSTRAINT FK_Renter_Lease FOREIGN KEY (LeaseId) REFERENCES dbo.Lease(LeaseID)
	ON DELETE NO ACTION
	ON UPDATE NO ACTION,
);
*/

