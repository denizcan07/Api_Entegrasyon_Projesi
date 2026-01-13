using ApiEntegrasyon.Context;
using ApiEntegrasyon.Repository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

public class DatabaseInitServices: IDatabaseInitServices
{
    private readonly IDbConnection _db;
    private readonly DapperContext _context;

    public DatabaseInitServices(IDbConnection db, DapperContext context)
    {
        _db = db;
        _context = context;
    }

    public async Task InitializeAsync()
    {

        using var masterConn = _context.CreateMasterConnection();

        await masterConn.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'ApiEntegrasyon')
            BEGIN
                CREATE DATABASE ApiEntegrasyon;
            END
        ");


        if (_db.State != ConnectionState.Open)
            _db.Open();

        using var tran = _db.BeginTransaction();
        try
        {    
            await _db.ExecuteAsync(@"
                IF OBJECT_ID('dbo.Users','U') IS NULL
                CREATE TABLE dbo.Users(
                    Id INT IDENTITY PRIMARY KEY,
                    Username NVARCHAR(100) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(256) NOT NULL,
                    IsActive BIT NOT NULL DEFAULT 1,
                    IsDeleted BIT NOT NULL DEFAULT 0,
                    CreatedBy NVARCHAR(100),
                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                );
             IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
                 INSERT INTO dbo.Users
                 (
                     Username,
                     PasswordHash,
                     IsActive,
                     IsDeleted,
                     CreatedBy,
                     CreatedAt
                 )
                 VALUES
                 (
                     'admin',
                     'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=',
                     1,
                     0,
                     'dct',
                     '2026-01-12 00:00:00.000'
                 );
            ", transaction: tran);



            await _db.ExecuteAsync(@"
                IF OBJECT_ID('dbo.Product','U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Product(
                        Id INT IDENTITY PRIMARY KEY,
                        Name NVARCHAR(200) NOT NULL,
                        Category NVARCHAR(100),
                        Price DECIMAL(18,2) NOT NULL,
                        IsActive BIT NOT NULL DEFAULT 1,
                        IsDeleted BIT NOT NULL DEFAULT 0,
                        CreatedBy NVARCHAR(100),
                        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                    );
                
                    SET IDENTITY_INSERT dbo.Product ON;
                
                    INSERT INTO dbo.Product
                        (Id, Name, Category, Price, IsActive, IsDeleted, CreatedBy, CreatedAt)
                    VALUES
                        (1, 'TEST',  'Kategori1', 500.00, 1, 0, 'dct', '2026-01-12 00:00:00.000'),
                        (2, 'TEST2', 'Kategori2', 800.00, 1, 0, 'dct', '2026-01-12 23:27:34.360'),
                        (3, 'TEST3', 'Kategori3', 750.00, 1, 0, 'dct', '2026-01-12 00:00:00.000'),
                        (4, 'TEST4', 'Kategori4', 550.00, 1, 0, 'dct', '2026-01-12 00:00:00.000'),
                        (5, 'TEST5', 'Kategori5', 525.00, 1, 0, 'dct', '2026-01-12 00:00:00.000');
                
                    SET IDENTITY_INSERT dbo.Product OFF;
                END
                ", transaction: tran);


            await _db.ExecuteAsync(@"IF OBJECT_ID('dbo.sp_user_get_by_username','P') IS NOT NULL DROP PROCEDURE dbo.sp_user_get_by_username;", transaction: tran);
            await _db.ExecuteAsync(@"
            CREATE PROCEDURE dbo.sp_user_get_by_username
                @Username NVARCHAR(100),
                @PasswordHash NVARCHAR(256)
            AS
            BEGIN
                SET NOCOUNT ON;
                SELECT * FROM dbo.Users
                WHERE Username = @Username
                  AND PasswordHash = @PasswordHash
                  AND IsActive = 1
                  AND IsDeleted = 0;
            END
            ", transaction: tran);
            
           await _db.ExecuteAsync(@"IF OBJECT_ID('dbo.sp_product_get_all','P') IS NOT NULL DROP PROCEDURE dbo.sp_product_get_all;", transaction: tran);
           await _db.ExecuteAsync(@"
            CREATE PROCEDURE dbo.sp_product_get_all
            AS
            BEGIN
                SET NOCOUNT ON;
                SELECT * FROM dbo.Product WHERE IsActive = 1 AND IsDeleted = 0;
            END
            ", transaction: tran);
            
            await _db.ExecuteAsync(@"IF OBJECT_ID('dbo.sp_product_get_by_id','P') IS NOT NULL DROP PROCEDURE dbo.sp_product_get_by_id;", transaction: tran);
            await _db.ExecuteAsync(@"
            CREATE PROCEDURE dbo.sp_product_get_by_id
                @Id INT
            AS
            BEGIN
                SET NOCOUNT ON;
                SELECT * FROM dbo.Product WHERE Id = @Id AND IsActive = 1 AND IsDeleted = 0;
            END
            ", transaction: tran);
            
            await _db.ExecuteAsync(@"IF OBJECT_ID('dbo.sp_product_insert','P') IS NOT NULL DROP PROCEDURE dbo.sp_product_insert;", transaction: tran);
            await _db.ExecuteAsync(@"
                       CREATE PROCEDURE [dbo].[sp_product_insert]
				@Name NVARCHAR(100),
				@Category NVARCHAR(100),
				@Price DECIMAL(18,2),
				@IsActive BIT,
				@IsDeleted BIT,
				@CreatedBy NVARCHAR(50),
				@CreatedAt DATETIME
			AS
			BEGIN
			    INSERT INTO dbo.Product
			    (
			        Name,
			        Category,
			        Price,
			        IsActive,
			        IsDeleted,
			        CreatedBy,
			        CreatedAt
			    )
			    VALUES
			    (
			        @Name,
			        @Category,
			        @Price,
			        @IsActive,
			        @IsDeleted,
			        @CreatedBy,
			        @CreatedAt
			    );
			END
            ", transaction: tran);
            
            await _db.ExecuteAsync(@"IF OBJECT_ID('dbo.sp_product_update','P') IS NOT NULL DROP PROCEDURE dbo.sp_product_update;", transaction: tran);
            await _db.ExecuteAsync(@"
            CREATE PROCEDURE dbo.sp_product_update
                    @Id INT,
                    @Name NVARCHAR(200),
                    @Category NVARCHAR(100),
                    @Price DECIMAL(18,2),
                    @IsActive BIT,
                    @IsDeleted BIT,
                    @CreatedBy NVARCHAR(100),
                    @CreatedAt DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;
                
                    UPDATE dbo.Product
                    SET
                        Name = @Name,
                        Category = @Category,
                        Price = @Price,
                        IsActive = @IsActive,
                        IsDeleted = @IsDeleted,
                        CreatedBy = @CreatedBy,
                        CreatedAt = @CreatedAt
                    WHERE Id = @Id;
                END

            ", transaction: tran);
            
            await _db.ExecuteAsync(@"IF OBJECT_ID('dbo.sp_product_delete','P') IS NOT NULL DROP PROCEDURE dbo.sp_product_delete;", transaction: tran);
            await _db.ExecuteAsync(@"
            CREATE PROCEDURE [dbo].[sp_product_delete]
                @Id INT
            AS
            BEGIN
            DELETE FROM dbo.Product
                WHERE Id = @Id;
            END
            ", transaction: tran);

            tran.Commit();

        }
        catch
        {
            tran.Rollback();
            throw;
        }
        finally
        {
            _db.Close();
        }
    }
}
