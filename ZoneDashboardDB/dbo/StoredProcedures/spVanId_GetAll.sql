CREATE PROCEDURE [dbo].[spVanId_GetAll]
AS
    begin 
        SELECT VanId, VanName, Blocked
        FROM dbo.[VanId];
    end