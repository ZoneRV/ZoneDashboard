CREATE PROCEDURE [dbo].[spVanId_Insert]
    @VanName varchar(7),
    @VanId varchar(24),
    @Url varchar(1024),
    @Blocked bit
AS
    begin 
        INSERT INTO dbo.[VanId] (VanName, VanId, Url, Blocked)
        values (@VanName, @VanId, @Blocked, @Url)
    end