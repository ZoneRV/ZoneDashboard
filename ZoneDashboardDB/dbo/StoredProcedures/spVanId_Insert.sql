CREATE PROCEDURE [dbo].[spVanId_Insert]
    @VanName varchar(7),
    @VanId varchar(24),
    @Blocked bit
AS
    begin 
        INSERT INTO dbo.[VanId] (VanName, VanId, Blocked)
        values (@VanName, @VanId, @Blocked)
    end