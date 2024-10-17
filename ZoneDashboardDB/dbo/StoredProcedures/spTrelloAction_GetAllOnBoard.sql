CREATE PROCEDURE [dbo].[spTrelloAction_GetAllOnBoard]
    @BoardId varchar(24)
AS
    begin 
        SELECT *
        FROM dbo.[TrelloAction]
        WHERE BoardId = @BoardId
        ORDER BY DateOffset;
    end