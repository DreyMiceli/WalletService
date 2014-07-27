USE [Wallet];
GO

SET IDENTITY_INSERT [dbo].[TransactionTypes] ON
INSERT INTO [dbo].[TransactionTypes] ([Id], [Type]) VALUES (1, N'Deposit')
INSERT INTO [dbo].[TransactionTypes] ([Id], [Type]) VALUES (2, N'Withdrawal')
SET IDENTITY_INSERT [dbo].[TransactionTypes] OFF
GO


SET IDENTITY_INSERT [dbo].[AccountStates] ON
INSERT INTO [dbo].[AccountStates] ([Id], [State]) VALUES (1, N'Open')
INSERT INTO [dbo].[AccountStates] ([Id], [State]) VALUES (2, N'Closed')
SET IDENTITY_INSERT [dbo].[AccountStates] OFF
GO

SET IDENTITY_INSERT [dbo].[Accounts] ON
INSERT INTO [dbo].[Accounts] ([ID], [UserID], [Balance], [AccountStateId], [DateCreated], [DateClosed]) VALUES (1, N'34dd23af-3e91-4289-b1a3-c71e4b1d44df@email.com', CAST(1000 AS Decimal(18, 0)), 1, N'2014-04-27 20:53:45', NULL)
INSERT INTO [dbo].[Accounts] ([ID], [UserID], [Balance], [AccountStateId], [DateCreated], [DateClosed]) VALUES (2, N'test@email.com', CAST(0 AS Decimal(18, 0)), 2, N'2014-04-27 20:53:45', N'2014-04-27 20:53:45')
SET IDENTITY_INSERT [dbo].[Accounts] OFF
GO