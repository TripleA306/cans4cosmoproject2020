import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import internal.GlobalVariable as GlobalVariable

/**
 * This tests the functionality of setting a Region from active to inactive and choosing to not delete the associated routes
 * Authors: Aaron Atkinson cst201
 */

//Click on the Region Nav bar to navigate to the Region Page
String RegionName = "Kyle's S90 Active Region"
String RouteName = "S90 Active Route"

//Click on the Region Nav bar to navigate to the Region Page
WebUI.click(findTestObject('Object Repository/Admin_Regions/btn_Region_Nav'))

//Verify that the region table, and the first 5 regions are present
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/RegionTable/tbl_RegionTable'), 5)

WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_item1'), 5)
WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_Item2'), 5)
WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_Item3'), 5)
WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_Item4'), 5)
WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_Item5'), 5)

//Verify that the region details pane exists
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/DetailsPaneDiv'), 5)

//Verify that the Region show all Button is visible
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_ShowAll'), 5)

//Click on the second Region on the table
WebUI.click(findTestObject('Admin_Regions/RegionTable/td_item2'))

//Verify that the details pane displays the fields
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_RegionName'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_RegionName'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_Frequency'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_Frequency'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_FirstCollection'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_FirstCollection'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_NextCollectionLabel'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_NextCollectionDate'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_Active'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/swt_Active'), 5)


//Click on the delete Region Button
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_DeleteRegion'))

//Click on the okay button on the delete modal
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/DeleteModal/btn_Modal_Okay'))

//Click on keep routes
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/DeleteModal/btn_Modal_KeepRoutes'))

//Verify that the Region is no longer displayed on the table
String itemRegionName = WebUI.getText(findTestObject('Object Repository/Admin_Regions/RegionTable/td_item2'))
WebUI.verifyNotMatch(itemRegionName, RegionName, false)

//Confirm that it is visible on the table if all Regions are shown
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_ShowAll'))
String showAllRegionName = WebUI.getText(findTestObject('Object Repository/Admin_Regions/RegionTable/td_item2'))
WebUI.verifyMatch(showAllRegionName, RegionName, false)

//Confirm that the route is still on the routes page
WebUI.click(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'))

//Click to Page two
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageTwo_NotActive'))

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'), 0)
String verifyRouteName = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'))
WebUI.verifyMatch(verifyRouteName, RouteName, false)



