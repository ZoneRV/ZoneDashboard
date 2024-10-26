CREATE PROCEDURE [dbo].[spVanId_GetAll]
AS
    begin 
        SELECT VanId, VanName, Url, Blocked
        FROM dbo.[VanId];
    end