/* drop the database if it exists */
print '' print '** dropping database library_db'
GO
IF EXISTS (SELECT 1 FROM master.dbo.sysdatabases WHERE name = 'library_db')
BEGIN
    DROP DATABASE [library_db]
END
GO

print '' print '** creating database library_db'
GO
CREATE DATABASE [library_db]
GO

print '' print '*** using database library_db'
GO
USE [library_db]
GO

/* ============================================
     Beginning of Creating Staff Tables
===============================================*/
print '' print '*** creating Staff table'
GO
CREATE TABLE [dbo].[Staff] (
    [StaffID]           [int] IDENTITY(1000000, 1) NOT NULL,
    [GivenName]         [nvarchar](50)             NOT NULL,
    [FamilyName]        [nvarchar](100)            NOT NULL,
    [PhoneNumber]       [nvarchar](11)             NOT NULL,
    [Email]             [nvarchar](250)            NOT NULL,
    [PasswordHash]      [nvarchar](100)            NOT NULL DEFAULT
        '9c9064c59f1ffa2e174ee754d2979be80dd30db552ec03e7e327e9b1a4bd594e',
    [Active]            [bit]                      NOT NULL DEFAULT 1,
   
    CONSTRAINT [pk_Staffid] PRIMARY KEY([StaffID] ASC),
    CONSTRAINT [ak_email] UNIQUE([Email] ASC)
)
GO

print '' print '*** create Role table'
GO
CREATE TABLE [dbo].[Role] (
    [RoleID]            [nvarchar](50)      NOT NULL,
    [Description]       [nvarchar](250)     NULL,
	
    CONSTRAINT [pk_roleid] PRIMARY KEY([RoleID] ASC)  
)
GO

print '' print '*** create StaffRole table'
GO
CREATE TABLE [dbo].[StaffRole] (
    [StaffID]           [int]               NOT NULL,
    [RoleID]            [nvarchar](50)      NOT NULL,

    CONSTRAINT [pk_Staff_roleid] PRIMARY KEY([StaffID], [RoleID]),
    CONSTRAINT [fk_Staff_role_Staffid] FOREIGN KEY([StaffID])
        REFERENCES[Staff]([StaffID]),
    CONSTRAINT [fk_Staff_role_roleid] FOREIGN KEY([RoleID])
        REFERENCES[Role]([RoleID])
)
GO
/* ============================================
     End of Creating Staff Tables 
     Beginning of Inserting Data into Staff Tables
===============================================*/
print '' print '*** inserting Staff test records'
GO
INSERT INTO [dbo].[Staff]
        ([GivenName], [FamilyName], [PhoneNumber], [Email])
    VALUES
        ('System', 'Admin', '00000000000', 'admin@root.com'),
        ('May', 'Lillian', '13194441112', 'may@center.com'),
        ('Emma', 'Williams', '13194441113', 'emma@center.com'),
		('Tommy', 'Mitt',  '13194441114', 'tommy@center.com'),
		('Tanner', 'Armstrong',  '13194441115', 'tanner@center.com'),
		('Jim', 'Badworker',  '13194441116', 'jim@center.com')
GO

/* Creating an inactive user */
print '' print '*** create an inactive user'
GO
UPDATE     [dbo].[Staff]
    SET    [Active] = 0
    WHERE  [Email] = 'jim@center.com'
GO

print '' print '*** inserting Role test records'
GO
INSERT INTO [dbo].[Role]
        ([RoleID], [Description])
    VALUES
        ('Admin', 'Manages users and user accounts.'),
		('Reservation', 'Manages book reservations.'),
        ('Maintenance', 'Repairs and maintains the books.'),
        ('Manager', 'Manages the library inventory.'),
		('CheckIn', 'Manages book checkin and inspection.'),
        ('No Access', 'Banned from everything.')
GO

print '' print '*** inserting StaffRole test records'
GO
INSERT INTO [dbo].[StaffRole]
        ([RoleID], [StaffID])
    VALUES
        ('Admin', 1000000 ),
		('Reservation', 1000001),
        ('Maintenance',1000002),
        ('Manager', 1000003),
		('CheckIn', 1000004),
        ('No Access', 1000005)
GO  
/* ============================================
    End of Inserting Data into Staff Tables
    Beginning of Staff Stored Procedures
===============================================*/

print '' print '*** creating procedure sp_authenticate_user'
GO
CREATE PROCEDURE [dbo].[sp_authenticate_user]
    (
        @Email          [nvarchar](250),
        @PasswordHash   [nvarchar](100)
    )
AS
    BEGIN
        SELECT  COUNT([StaffID])
        FROM    [Staff]
        WHERE   [Email] = @Email
          AND   [PasswordHash] = @PasswordHash
          AND   [Active] = 1
    END
GO

print '' print '*** creating procedure sp_select_user_by_email'
GO
CREATE PROCEDURE [dbo].[sp_select_user_by_email]
    (
        @Email      [nvarchar] (250)
    )
AS
    BEGIN
        SELECT [StaffID], [GivenName], [FamilyName], [PhoneNumber],
                    [Email], [Active]
        FROM    [Staff]
        WHERE   [Email] = @Email
    END
GO

print '' print '*** creating procedure sp_select_roles_by_StaffID'
GO
CREATE PROCEDURE [dbo].[sp_select_roles_by_StaffID]
    (
        @StaffID     [int]
    )
AS
    BEGIN
        SELECT [StaffID], [RoleID]
        FROM    [StaffRole]
        WHERE   [StaffID] = @StaffID
    END
GO

print '' print '*** creating procedure sp_update_passwordhash_by_email'
GO
CREATE PROCEDURE [dbo].[sp_update_passwordhash_by_email]
    (
        @Email              [nvarchar] (250),
        @NewPasswordHash    [nvarchar] (100),
        @OldPasswordHash    [nvarchar] (100)
    )
AS
    BEGIN
        UPDATE      [Staff]
        SET         [PasswordHash] = @NewPasswordHash
        WHERE       [PasswordHash] = @OldPasswordHash
          AND       [Email] = @Email


        RETURN @@ROWCOUNT
    END
GO

print '' print '*** creating procedure sp_select_all_staff'
GO
CREATE PROCEDURE [dbo].[sp_select_all_staff]
AS
    BEGIN
        SELECT      s.[StaffID], s.[GivenName], s.[FamilyName], s.[PhoneNumber],
                        s.[Email], s.[Active], sr.[RoleID]
        FROM        [dbo].[Staff] s
		LEFT JOIN [dbo].[StaffRole] sr ON s.[StaffID] = sr.[StaffID]
        ORDER BY    [FamilyName]
    END
GO

print '' print '*** creating procedure sp_update_Staff_email'
GO
CREATE PROCEDURE [dbo].[sp_update_Staff_email]
    (
		@NewEmail		[nvarchar](250),
		@OldEmail		[nvarchar](250),
		@PasswordHash	[nvarchar](100)
    )
AS
    BEGIN
        UPDATE		[Staff]
		SET 		[Email] = @NewEmail
		WHERE		[Email] = @OldEmail
		 AND		[PasswordHash] = @PasswordHash
		 
		RETURN @@ROWCOUNT
    END
GO

print '' print '*** creating sp_insert_staff'
GO
CREATE PROCEDURE [dbo].[sp_insert_staff]
	(
		@GivenName		[nvarchar](50),
		@FamilyName		[nvarchar](100),
		@PhoneNumber	[nvarchar](11),
		@Email			[nvarchar](250),
		@RoleID			[nvarchar](50)
	)
AS
	BEGIN
		INSERT INTO [dbo].[Staff]
			([GivenName], [FamilyName], [PhoneNumber], [Email])
		VALUES
			(@GivenName, @FamilyName, @PhoneNumber, @Email)
		DECLARE @StaffID [int];
		SET @StaffID = (SELECT SCOPE_IDENTITY());
		
		INSERT INTO [dbo].[StaffRole] (StaffID, RoleID)
		VALUES (@StaffID, @RoleID);
	END
GO

print '' print '*** creating sp_insert_role_to_staff'
GO
CREATE PROCEDURE [dbo].[sp_insert_role_to_staff]
	(
		@StaffID 	[int],
		@RoleID		[nvarchar](50)
	)
AS
	BEGIN
		IF NOT EXISTS 
		(
			SELECT 1 
			FROM [dbo].[StaffRole] 
			WHERE StaffID = @StaffID
				AND RoleID = @RoleID
		)
		BEGIN
			INSERT INTO [dbo].[StaffRole]
				([StaffID], [RoleId])
			VALUES
				(@StaffID, @RoleID);
		END
	END
GO

print '' print '*** creating sp_remove_role_from_staff'
GO
CREATE PROCEDURE [dbo].[sp_remove_role_from_staff]
    (
        @StaffID   [int],
        @RoleID    [nvarchar](50)   
	)
AS
	BEGIN
        IF EXISTS 
        (
            SELECT 1 
            FROM [dbo].[StaffRole] 
            WHERE StaffID = @StaffID
                AND RoleID = @RoleID
        )
        BEGIN
           
            DELETE FROM [dbo].[StaffRole]
            WHERE StaffID = @StaffID
                AND RoleID = @RoleID;
        END
    END
GO

print '' print '*** creating sp_activate_staff_and_restore_roles'
GO
CREATE PROCEDURE [dbo].[sp_activate_staff]
    (
        @StaffID   [int]  
	)
AS
	BEGIN
		UPDATE [dbo].[Staff]
		SET [Active] = 1
		WHERE [StaffID] = @StaffID; 
		
		DELETE FROM [dbo].[StaffRole]
		WHERE [StaffID] = @StaffID
			AND [RoleID] = 'No Access';
    END 
GO

print '' print '*** creating sp_deactivate_staff_and_remove_role'
GO
CREATE PROCEDURE [dbo].[sp_deactivate_staff_and_remove_role]
    (
        @StaffID   [int]  
	)
AS
	BEGIN
        UPDATE [dbo].[Staff]
		SET [Active] = 0
		WHERE [StaffID] = @StaffID;
		
		DELETE FROM [dbo].[StaffRole]
		WHERE [StaffID] = @StaffID;
		
		IF NOT EXISTS
			(
				SELECT 1
				FROM [dbo].[Role] 
				WHERE [RoleID] = 'No Access'
			)
		BEGIN
			RETURN;
		END
		
		INSERT INTO [dbo].[StaffRole] 
			([StaffID], [RoleID])
		VALUES
			(@StaffID, 'No Access');
	END
GO

/*==================================================
        End of Staff Stored Procedures
        Beginning of Book and Related Tables
=====================================================*/       
print '' print '*** creating Status table'
GO
CREATE TABLE [dbo].[Status] (
    [StatusID]      [nvarchar](50)          NOT NULL,
    [Description]   [nvarchar](500)         NOT NULL DEFAULT '',
 
    CONSTRAINT [pk_statusid] PRIMARY KEY([StatusID] ASC)  
)
GO

print '' print '*** creating Author table'
GO
CREATE TABLE [dbo].[Author] (
    [AuthorID]       [nvarchar](50)          NOT NULL,
	[GivenName]      [nvarchar](50)          NOT NULL,
    [FamilyName]     [nvarchar](100)         NOT NULL,
	[Biography]		 [nvarchar](500)         NOT NULL,
	
    CONSTRAINT [pk_authorid] PRIMARY KEY([AuthorID] ASC)
)
GO

print '' print '*** creating Genre table'
GO
CREATE TABLE [dbo].[Genre] (
    [GenreID]       [nvarchar](50)          NOT NULL,
    [GenreName]     [nvarchar](100)         NOT NULL,
	
    CONSTRAINT [pk_genreid] PRIMARY KEY([GenreID] ASC)  
)
GO

print '' print '*** creating Book table'
GO
CREATE TABLE [dbo].[Book] (
    [BookID]       	[int]			        NOT NULL,
	[StatusID]		[nvarchar](50)			NOT NULL,
	[AuthorID]		[nvarchar](50)			NOT NULL,
	[GenreID]		[nvarchar](50)			NOT NULL,
    [Title]        	[nvarchar](100)         NOT NULL,
	[PublishedYear] [int]					NOT NULL,
	[Active]        [bit]               NOT NULL DEFAULT 1,

    CONSTRAINT [pk_bookid] PRIMARY KEY([BookID] ASC),
	CONSTRAINT [fk_statusid] FOREIGN KEY([StatusID]) 
		REFERENCES [dbo].[Status]([StatusID]),
	CONSTRAINT [fk_authorid] FOREIGN KEY([AuthorID]) 
		REFERENCES [dbo].[Author]([AuthorID]),
	CONSTRAINT [fk_genreid] FOREIGN KEY([GenreID]) 
		REFERENCES [dbo].[Genre]([GenreID])
)
GO

print '' print '*** creating Members table'
GO
CREATE TABLE [dbo].[Members] (
    [MemberID] 		[int] IDENTITY(1,1) NOT NULL,       
    [GivenName] 	[nvarchar](50) NOT NULL,              
    [FamilyName] 	[nvarchar](100) NOT NULL,               
    [Email] 		[nvarchar](250) NOT NULL,                 
    [PhoneNumber] 	[nvarchar](11),                     
    [Active] 		[bit] NOT NULL DEFAULT 1 

	CONSTRAINT [pk_memberid] PRIMARY KEY([MemberID]  ASC)
)
GO

print '' print '*** creating Fines table'
GO
CREATE TABLE [dbo].[Fines] (
    [FineID]        [int] IDENTITY(1,1) NOT NULL,              
    [MemberID]      [int] NOT NULL,                             
    [Amount]        [decimal](10, 2) NOT NULL,                 
    [IssueDate]     [datetime] NOT NULL,                                          
    [Paid]          [bit] NOT NULL DEFAULT 0,                                           
    
    CONSTRAINT [pk_fineid] PRIMARY KEY([FineID] ASC),           
    CONSTRAINT [fk_memberid] FOREIGN KEY([MemberID])            
        REFERENCES [dbo].[Members]([MemberID])
)
GO

print '' print '*** creating ReservedBooks table'
GO
CREATE TABLE [dbo].[ReservedBooks] (
    [ReservedBooksID]  		[int] 			IDENTITY(1,1) NOT NULL,              
    [MemberID] 				[int]			NOT NULL,
	[BookID]				[int]			NOT NULL,
	[StaffID]				[int]			NOT NULL,
	[ReservationDate]		[datetime]		NOT NULL,
	[StatusID]				[nvarchar](50)  NOT NULL DEFAULT '3'	
	
    CONSTRAINT [pk_reservedbooksid] PRIMARY KEY([ReservedBooksID] ASC),           
    CONSTRAINT [fk_reserved_memberid] FOREIGN KEY([MemberID])            
        REFERENCES [dbo].[Members]([MemberID]),
	CONSTRAINT [fk_bookid] FOREIGN KEY([BookID])            
        REFERENCES [dbo].[Book]([BookID]),
	CONSTRAINT [fk_reserved_staffid] FOREIGN KEY([StaffID])                        
        REFERENCES [dbo].[Staff]([StaffID])
)
GO

print '' print '*** creating Maintenance table'
GO
CREATE TABLE [dbo].[Maintenance] (
	[MaintenanceID]		[int]					IDENTITY(1,1) NOT NULL,
    [BookID]       		[int]	         		NOT NULL,
	[StaffID]			[int]					NOT NULL,
	[StatusID]			[nvarchar](50)			NOT NULL DEFAULT '4',
	[MaintenanceDate]	[datetime]				NOT NULL,
	[Description]		[nvarchar](250)			NOT NULL,
	
    CONSTRAINT [pk_maintenance] PRIMARY KEY([MaintenanceID] ASC),
    CONSTRAINT [fk_maintenance_bookid] FOREIGN KEY([BookID]) 
		REFERENCES[dbo].[Book]([BookID]),
    CONSTRAINT [fk_maintenance_staffid] FOREIGN KEY([StaffID]) 
		REFERENCES[dbo].[Staff]([StaffID])
)
GO
/*==================================================
        End of Creating Book and Related Tables 
        Beginning of Inserting Data
=====================================================*/ 

/* Member related inserting data */
print '' print '*** inserting Members into the Members table'
GO
INSERT INTO [dbo].[Members] ([GivenName], [FamilyName], [Email], [PhoneNumber], [Active])
VALUES 
    ('John', 'Doe', 'john@email.com', '12345678901', 1),
    ('Jane', 'Smith', 'jane@email.com', '12345678902', 1),
    ('Alice', 'Johnson', 'alice@email.com', '12345678903', 1),
    ('Bob', 'Williams', 'bob@email.com', '12345678904', 1),
	('Jill', 'White','jill@email', '12345678905', 1),
    ('Charlie', 'Brown', 'charlie@email.com', '12345678905', 1),
	('Emily', 'Davis', 'emily@email.com', '12345678906', 1),
    ('Michael', 'Clark', 'michael@email.com', '12345678907', 1),
    ('Sarah', 'Miller', 'sarah@email.com', '12345678908', 1),
    ('David', 'Martinez', 'david@email.com', '12345678909', 1),
    ('Sophia', 'Garcia', 'sophia@email.com', '12345678910', 1),
    ('Daniel', 'Anderson', 'daniel@email.com', '12345678911', 1),
    ('Olivia', 'Harris', 'olivia@email.com', '12345678912', 1),
    ('Matthew', 'Robinson', 'matthew@email.com', '12345678913', 1),
    ('Chloe', 'Moore', 'chloe@email.com', '12345678914', 1),
    ('James', 'Taylor', 'james@email.com', '12345678915', 1),
    ('Ava', 'Lee', 'ava@email.com', '12345678916', 1),
    ('Christopher', 'King', 'christopher@email.com', '12345678917', 1),
    ('Mia', 'Lopez', 'mia@email.com', '12345678918', 1),
    ('Alexander', 'Hill', 'alexander@email.com', '12345678919', 1),
    ('Isabella', 'Scott', 'isabella@email.com', '12345678920', 1);
GO

print '' print '*** inserting Fines test records'
GO
INSERT INTO [dbo].[Fines] ([MemberID], [Amount], [IssueDate], [Paid])
VALUES
    (1, 25.50, '2024-11-01',  0 ),
    (2, 15.00, '2024-11-05', 0 ),
    (3, 30.00, '2024-11-07', 0)
GO

print '' print '*** inserting Status test records'
GO
INSERT INTO [dbo].[Status]
        ([StatusID], [Description])
    VALUES
        ('1', 'Available'),
        ('2', 'Checked In'),
		('3', 'Reserved'),
		('4', 'In Maintenance')     
GO 

print '' print '*** inserting Author test records'
GO
INSERT INTO [dbo].[Author]
        ([AuthorID], [GivenName], [FamilyName], [Biography])
    VALUES
        ('A1', 'Jonny', 'Dee', 'An experienced author in mystery novels.'),
        ('A2', 'Willy', 'Smith', 'Specializes in science fiction works.'),
		('A3', 'Dill', 'Shawn', 'An experienced author in humor.'),
		('A4', 'Shawn', 'Ken', 'Specializes in thriller works.'),
		('A5', 'Rose', 'Heart', 'An experienced author in romance.'),
		('A6', 'Matt', 'Lome', 'Specializes in non fiction'),
		('A7', 'Jane', 'Black', 'Specializes in science fiction works.'),
		('A8', 'Emily', 'Stone', 'A celebrated author in historical fiction.'),
        ('A9', 'Michael', 'Green', 'Known for his engaging poetry collections.'),
        ('A10', 'Sophia', 'Harper', 'Writes captivating fantasy series.'),
        ('A11', 'David', 'Cross', 'A well-known humorist and essayist.'),
        ('A12', 'Isabella', 'Lee', 'Famous for her inspiring self-help books.'),
        ('A13', 'Ethan', 'Reed', 'Renowned for his suspense and crime novels.'),
        ('A14', 'Mia', 'Clark', 'Focuses on heartfelt romance stories.'),
        ('A15', 'Alexander', 'Wright', 'Expert in political and social commentaries.'),
        ('A16', 'Olivia', 'Adams', 'Specializes in children’s adventure books.'),
        ('A17', 'Christopher', 'Hill', 'Writes novels blending science fiction with mystery.')		
GO 

print '' print '*** inserting Genre test  records'
GO
INSERT INTO [dbo].[Genre]
        ([GenreID], [GenreName])
    VALUES
        ('G1', 'Mystery'),
        ('G2', 'Science Fiction'),
		('G3', 'Humor'),
		('G4', 'Thriller'),
		('G5', 'Romance'),
		('G6', 'Non Fiction'),
		('G7', 'Fiction'),
		('G8', 'Poetry'),
		('G9', 'Fantasy'),
		('G10', 'History'),
		('G11', 'Adventure'),
        ('G12', 'Biography'),
        ('G13', 'Drama'),
        ('G14', 'Self-Help'),
        ('G15', 'Children’s Literature'),
        ('G16', 'Satire'),
        ('G17', 'Crime'),
        ('G18', 'Historical Fiction'),
        ('G19', 'Science'),
        ('G20', 'Art and Design');
GO 

print '' print '*** inserting Book test records'
GO
INSERT INTO [dbo].[Book]
        ([BookID], [StatusID], [AuthorID], [GenreID], [Title], [PublishedYear], [Active])
    VALUES
        ('1', '1', 'A1', 'G1', 'Mystery Night', 2022, 1),
        ('2', '1', 'A2', 'G2', 'Science Fiction', 2023, 1),
		('3', '2', 'A3', 'G3', 'Dad Jokes', 2024, 1),
		('4', '2', 'A4', 'G4', 'Thill in the Park', 2020, 1),
		('5', '3', 'A5', 'G5', 'Romanic Story', 2019, 1),
		('6', '3', 'A6', 'G6', 'Biography about Matt', 2018, 1),
		('7', '4', 'A3', 'G3', 'Cat Jokes', 2019, 1),
		('8', '4', 'A3', 'G3', 'Dog Jokes', 2017, 1)
GO 

print '' print '*** inserting ReservedBooks test records'
GO
	INSERT INTO [dbo].[ReservedBooks]
		([MemberID],[BookID],[StaffID],[ReservationDate],[StatusID])
	VALUES
		(4, '5', 1000001, '2024-11-15', '3'),
		(5, '6', 1000001, '2024-11-15', '3')
GO

print '' print '*** inserting Maintenance test records'
GO
	INSERT INTO [dbo].[Maintenance]
		([BookID],[StaffID],[MaintenanceDate],[StatusID], [Description])
	VALUES
		('7', 1000002, '2024-11-12', '4', 'Cleaning book cover'),
		('8', 1000002, '2024-11-13', '4', 'Repairing pages')	
GO
/*===============================================
        End of Inserting Data
        Beginning of Book stored procedure
================================================*/

/* Selecting Stored Procedures */
print '' print '*** creating procedure sp_select_available_books'
GO
CREATE PROCEDURE [dbo].[sp_select_available_books]
AS
	BEGIN
		SELECT 
			[BookID], 
			[StatusID], 
			a.[GivenName] AS AuthorGivenName, 
			a.[FamilyName] AS AuthorFamilyName,
			g.[GenreName], 
			[Title], 
			[PublishedYear], 
			[Active]
		FROM [dbo].[Book] b
		INNER JOIN dbo.Author a ON b.AuthorID = a.AuthorID
		INNER JOIN dbo.Genre g ON b.GenreID = g.GenreID
		WHERE [StatusID] = '1'
			AND [Active] = 1
		ORDER BY [BookID]
	END
GO

print '' print '*** creating procedure sp_select_reserved_books'
GO
CREATE PROCEDURE [dbo].[sp_select_reserved_books]
AS
	BEGIN
		SELECT
			rb.[ReservedBooksID],
			rb.[MemberID],
			rb.[BookID],
			rb.[StaffID],
			rb.[ReservationDate],
			rb.[StatusID],
			m.[GivenName] AS MemberGivenName,
			m.[FamilyName] AS MemberFamilyName,
			b.[Title]
		FROM
			[dbo].[ReservedBooks] rb
		JOIN 
			[dbo].[Members] m ON rb.MemberID = m.MemberID
		JOIN 
		[dbo].[Book] b ON rb.BookID = b.BookID
		JOIN 
			[dbo].[Staff] s ON rb.StaffID = s.StaffID
		WHERE
			rb.StatusID = '3';
	END
GO

print '' print '*** creating procedure sp_select_maintenance_books'
GO
CREATE PROCEDURE [dbo].[sp_select_maintenance_books]
AS
	BEGIN
		SELECT
			mt.[MaintenanceID],
			mt.[BookID],
			mt.[StaffID],
			mt.[MaintenanceDate],
			mt.[StatusID],
			mt.[Description],
			b.[Title]
		FROM
			[dbo].[Maintenance] mt
		JOIN 
		[dbo].[Book] b ON mt.BookID = b.BookID
		JOIN 
			[dbo].[Staff] s ON mt.StaffID = s.StaffID
		WHERE
			mt.StatusID = '4';
	END
GO

print '' print '*** creating procedure sp_select_checkin_books'
GO
CREATE PROCEDURE [dbo].[sp_select_checkin_books]
AS
	BEGIN
		SELECT 
			[BookID], 
			[StatusID], 
			a.[GivenName] AS AuthorGivenName,
			a.[FamilyName] AS AuthorFamilyName,
			g.[GenreName],
			[Title], 
			[PublishedYear],
			[Active]
		FROM [dbo].[Book] b
		INNER JOIN dbo.Author a ON b.AuthorID = a.AuthorID
		INNER JOIN dbo.Genre g ON b.GenreID = g.GenreID
		WHERE [StatusID] = '2'
			AND [Active] = 1
		ORDER BY [BookID]
	END
GO

print '' 
print '*** creating procedure sp_select_all_books'
GO
CREATE PROCEDURE [dbo].[sp_select_all_books]
AS
BEGIN
    SELECT 
        [BookID], 
        [StatusID], 
        a.[GivenName] AS AuthorGivenName,
        a.[FamilyName] AS AuthorFamilyName,
        g.[GenreName],
        [Title], 
        [PublishedYear],
        [Active]
    FROM [dbo].[Book] b
    INNER JOIN dbo.Author a ON b.AuthorID = a.AuthorID
    INNER JOIN dbo.Genre g ON b.GenreID = g.GenreID
    ORDER BY [BookID]
END
GO

/* Selecting Authors */
print '' print '*** creating procedure sp_select_all_authors'
GO
CREATE PROCEDURE [dbo].[sp_select_all_authors]
AS
BEGIN
    SELECT 
        [AuthorID],
        [GivenName],
        [FamilyName],
        [Biography]
    FROM [dbo].[Author];
END
GO

/* Selecting Genres */
print '' print '*** creating procedure sp_select_all_genres'
GO
CREATE PROCEDURE [dbo].[sp_select_all_genres]
AS
BEGIN
    SELECT 
        [GenreID],
        [GenreName]
    FROM [dbo].[Genre];
END
GO

/* Updating Stored Procedures */
print '' 
print '*** creating procedure sp_update_checkin_to_available_book'
GO
CREATE PROCEDURE [dbo].[sp_update_checkin_to_available_book]
	(	
		@BookID [int]
	)
AS
BEGIN
    UPDATE [dbo].[Book]
    SET [StatusID] = '1'
    WHERE [BookID] = @BookID AND [StatusID] = '2'; 
END 
GO

print '' print '*** creating procedure sp_update_reserved_to_checkin'
GO
CREATE PROCEDURE [dbo].[sp_update_reserved_to_checkin]
	(
		@BookID [int]
	)
AS
	BEGIN 
		DECLARE @ReservedStatusID [nvarchar](50) = '3';
		DECLARE @CheckInStatusID [nvarchar](50) = '2';
		
		IF EXISTS 
		(
			SELECT 1
			FROM [dbo].[Book]
			WHERE [BookID] = @BookID
			  AND [StatusID] = @ReservedStatusID
		)
		BEGIN 
			UPDATE [dbo].[Book]
			SET [StatusID] = @CheckInStatusID
			WHERE [BookID] = @BookID;
			
			DELETE FROM [dbo].[ReservedBooks]
			WHERE [BookID] = @BookID
		END
	END
GO

print '' print '*** creating procedure sp_update_available_to_reserve_book'
GO
CREATE PROCEDURE [dbo].[sp_update_available_to_reserve_book]
	(
		@BookID [int],
		@StaffID [int],
		@MemberID [int],
		@ReservationDate [datetime]
	)
AS
BEGIN
    UPDATE [dbo].[Book]
    SET [StatusID] = '3'
    WHERE [BookID] = @BookID AND [StatusID] = '1';

    IF @@ROWCOUNT > 0
    BEGIN
        INSERT INTO [dbo].[ReservedBooks]
            ([BookID], [StaffID], [MemberID], [ReservationDate] )
        VALUES
            (@BookID, @StaffID, @MemberID, @ReservationDate );
    END
END
GO

print '' print '*** creating procedure sp_update_checkin_to_maintenance_book'
GO
CREATE PROCEDURE [dbo].[sp_update_checkin_to_maintenance_book]
	(
		@BookID [int],
		@StaffID [int],
		@MaintenanceDate [datetime],
		@Description [nvarchar](250)
	)
AS
BEGIN
    UPDATE [dbo].[Book]
    SET [StatusID] = '4'
    WHERE [BookID] = @BookID AND [StatusID] = '2';

    IF @@ROWCOUNT > 0
    BEGIN
        INSERT INTO [dbo].[Maintenance]
            ([BookID], [StaffID], [MaintenanceDate], [Description] )
        VALUES
            (@BookID, @StaffID, @MaintenanceDate, @Description );
    END
END
GO

print '' print '*** creating procedure sp_update_maintenace_to_available'
GO
CREATE PROCEDURE [dbo].[sp_update_maintenace_to_available]
	(
		@BookID [int]
	)
AS
	BEGIN 
		DECLARE @MaintenanceStatusID [nvarchar](50) = '4';
		DECLARE @AvailableStatusID [nvarchar](50) = '1';
		
		IF EXISTS 
		(
			SELECT 1
			FROM [dbo].[Book]
			WHERE [BookID] = @BookID
			  AND [StatusID] = @MaintenanceStatusID
		)
		BEGIN 
			UPDATE [dbo].[Book]
			SET [StatusID] = @AvailableStatusID
			WHERE [BookID] = @BookID;
			
			DELETE FROM [dbo].[Maintenance]
			WHERE [BookID] = @BookID
		END
	END
GO

/* Inserting Stored Procedures */
print '' print '*** creating procedure sp_insert_book'
GO
CREATE PROCEDURE [dbo].[sp_insert_book]
    @BookID           [int],
    @StatusID         [nvarchar](50),
    @AuthorID         [nvarchar](50),
    @GenreID          [nvarchar](50),
    @Title            [nvarchar](100),
    @PublishedYear    [int]
AS
BEGIN
    INSERT INTO [dbo].[Book] 
        ([BookID], [StatusID], [AuthorID], [GenreID], [Title], [PublishedYear])
    VALUES
        (@BookID, @StatusID, @AuthorID, @GenreID, @Title, @PublishedYear)
END
GO
/*===============================================
        End of  Stored Procedures
        Beginning of Member Fine Stored Procedures
================================================*/

/* Selecting Stored Procedures */

print '' print '*** creating sp_insert_member'
GO
CREATE PROCEDURE [dbo].[sp_insert_member]
	(
		@GivenName	[nvarchar](50),
		@FamilyName		[nvarchar](100),
		@PhoneNumber	[nvarchar](11),
		@Email			[nvarchar](250)
	)
AS
	BEGIN
		INSERT INTO [dbo].[Members]
			([GivenName], [FamilyName], [PhoneNumber], [Email])
		VALUES
			(@GivenName, @FamilyName, @PhoneNumber, @Email)
		DECLARE @MemberID [int];
		SET @MemberID = (SELECT SCOPE_IDENTITY());
		
	END
GO

	
print '' print '*** creating sp_select_fines'
GO
CREATE PROCEDURE [dbo].[sp_select_fines]
AS
BEGIN

    SELECT 
        f.[FineID], 
        m.[GivenName] AS MemberGivenName,
		m.[FamilyName] AS MemberFamilyName,
		m.[Email] AS MemberEmail,
        f.[Amount], 
        f.[IssueDate], 
        f.[Paid]
    FROM 
        [dbo].[Fines] f
	INNER JOIN 
		[dbo].Members m ON f.[MemberID] = m.[MemberID]
END
GO

print '' print '*** creating procedure sp_select_all_members'
GO
CREATE PROCEDURE [dbo].[sp_select_all_members]
AS
BEGIN
    SELECT [MemberID], [GivenName], [FamilyName], [Email],[PhoneNumber]
    FROM [dbo].[Members]
    WHERE [Active] = 1
    ORDER BY [MemberID]
END
GO


/* Updating Stored Procedure */
print '' print '*** creating sp_update_fine_as_paid'
GO
CREATE PROCEDURE [dbo].[sp_mark_fine_as_paid]
    @FineID [int]     
AS
BEGIN
    UPDATE [dbo].[Fines]
    SET 
        [Paid] = 1                     
    WHERE 
        [FineID] = @FineID                 
END
GO

/* Inserting Stored Procedure */
print '' print '*** creating sp_insert_fine'
GO
CREATE PROCEDURE [dbo].[sp_insert_fine]
    @MemberID [int],
	@Amount [decimal](10,2),
	@IssueDate [datetime]
AS
BEGIN
	INSERT INTO [dbo].[Fines]
	( [MemberID], [Amount], [IssueDate], [Paid])
	VALUES
		(@MemberID, @Amount, @IssueDate, 0)
END
GO

















