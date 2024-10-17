CREATE PROCEDURE [dbo].[spVanId_Block]
    @VanName varchar(7),
    @Blocked bit
AS
    begin 
        UPDATE dbo.VanId
        SET Blocked = @Blocked
        WHERE VanName = @VanName;
    end