CREATE PROCEDURE [dbo].[spVanId_Update]
    @VanName varchar(7),
    @VanId varchar(24),
    @Blocked bit
AS
    begin 
        UPDATE dbo.VanId
        SET VanId = @VanId, Blocked = @Blocked
        WHERE VanName = @VanName;
    end