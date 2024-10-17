CREATE PROCEDURE [dbo].[spTrelloAction_DeleteOnBoard]
    @BoardId varchar(24)
AS
    begin 
        Delete
        FROM dbo.[TrelloAction]
        WHERE BoardId = @BoardId;
    end