IF NOT EXISTS (SELECT 1 FROM dbo.[VanId])
Begin
	INSERT INTO dbo.[VanId] (VanName, VanId)
	values ('testvan', '1234')
END