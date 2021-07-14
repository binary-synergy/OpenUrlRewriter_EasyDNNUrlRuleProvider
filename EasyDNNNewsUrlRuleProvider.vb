Imports Microsoft.VisualBasic
Imports Satrabel.HttpModules.Provider
Imports EasyDNNSolutions.Modules.EasyDNNNews
Imports DotNetNuke.Entities.Portals
Imports EasyDNNSolutions.Modules.EasyDNNNews.NewsDataController
Imports System.Data.SqlClient
Imports DotNetNuke.Entities.Modules

Public Class EasyDNNNewsUrlRuleProvider
    Inherits UrlRuleProvider

    Public Overrides Function GetRules(PortalId As Integer) As List(Of UrlRule)

        Dim portal As PortalInfo = (New PortalController).GetPortal(PortalId)
        Dim rules As New List(Of UrlRule)
        Dim dicSecondaryLocales As List(Of Locale) = LocaleController.Instance.GetLocales(PortalId).Values.Where(Function(l) l.Code <> portal.DefaultLanguage).ToList
        Dim blogModules As New Dictionary(Of Integer, Integer) ' module id to tab id

        'Dim userInfo = UserController.Instance.GetCurrentUserInfo()
        'Dim newsModuleSettings As SettingsController.NewsModuleSettings = New SettingsController.NewsModuleSettings()
        'Dim permissions As PermissionsController.Permissions = New PermissionsController.Permissions()
        ''Dim ps As PortalSettings = (New PortalController).Instance.GetCurrentPortalSettings()
        'Dim ps As PortalSettings = (New UserControlBase).PortalSettings
        'If ((Not IsNothing(userInfo)) And (Not IsNothing(PortalSettings.Current))) Then
        '    Dim con As EasyDNNNewsController = New EasyDNNNewsController()
        '    Dim con1 As NewsDataController = New NewsDataController(PortalSettings.Current, userInfo, False)
        '    Dim con2 As NewsDataDB
        '    Dim newArticle As NewsArticles = con1.DisplayListOnlyArticles(0, newsModuleSettings, permissions, 1, 100, -1, 0)
        'End If

        Dim Result As UrlDetail

        Using connection As SqlConnection = EDNNewsConnectionManager.GetDBConnection()

            Using sqlCommand As SqlCommand = New SqlCommand("EasyDNN_GetDetailsForRewiteUrl", connection)
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure
                'sqlCommand.Parameters.Add(New SqlParameter("@OE", YourValue)

                Using sqlDataReader As SqlDataReader = sqlCommand.ExecuteReader()
                    If (sqlDataReader.HasRows) Then
                        While (sqlDataReader.Read())
                            Result = New UrlDetail()
                            Result.ModuleID = sqlDataReader.GetInt32(0)
                            Result.TabId = sqlDataReader.GetInt32(1)
                            Result.ArticleID = sqlDataReader.GetInt32(2)
                            Result.Title = sqlDataReader.GetString(3)
                            Result.CategoryID = sqlDataReader.GetInt32(4)
                            Result.CategoryName = sqlDataReader.GetString(5)
                            Result.UserID = sqlDataReader.GetInt32(6)
                            Result.DisplayName = sqlDataReader.GetString(7)
                            Result.TagID = sqlDataReader.GetInt32(8)
                            Result.TagName = sqlDataReader.GetString(9)
                            Result.DateAdded = sqlDataReader.GetString(10)
                            Result.Parameters = sqlDataReader.GetString(11)

                            If Result.ArticleID > 0 Then
                                rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = Result.Parameters, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(Result.Title), .TabId = Result.TabId, .RemoveTab = False})
                            ElseIf Result.CategoryID > 0 Then
                                rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = Result.Parameters, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(Result.CategoryName), .TabId = Result.TabId, .RemoveTab = False})
                            ElseIf Result.UserID > 0 Then
                                rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = Result.Parameters, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(Result.DisplayName), .TabId = Result.TabId, .RemoveTab = False})
                            ElseIf Result.TagID > 0 Then
                                rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = Result.Parameters, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(Result.TagName), .TabId = Result.TabId, .RemoveTab = False})
                            ElseIf (Not String.IsNullOrEmpty(Result.DateAdded)) Then
                                rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = Result.Parameters, .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl(Result.DateAdded), .TabId = Result.TabId, .RemoveTab = False})
                            End If

                        End While
                    End If
                    sqlDataReader.Close()
                End Using
            End Using
        End Using

        'rules.Add(New UrlRule With {.Action = UrlRuleAction.Rewrite, .Parameters = "ArtMID=1710&ArticleID=70", .RuleType = UrlRuleType.Module, .Url = UrlRuleProvider.CleanupUrl("clisp-studio-release-21010"), .TabId = 0, .RemoveTab = False})

        Return rules

    End Function

End Class

Public Class UrlDetail
    Public ModuleID As Integer
    Public TabId As Integer
    Public ArticleID As Integer
    Public Title As String
    Public CategoryID As Integer
    Public CategoryName As String
    Public UserID As Integer
    Public DisplayName As String
    Public TagID As Integer
    Public TagName As String
    Public DateAdded As String
    Public Parameters As String
End Class