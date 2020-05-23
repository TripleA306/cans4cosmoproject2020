import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import internal.GlobalVariable as GlobalVariable

//Tests that if the table is not in Show All mode and a route is changed to Completed,
//the Route is not shown in the table after changes are saved


//Verify current Page is 1
WebUI.verifyElementText(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageOne_Active'), "1")

//Verify that there is a Route Name header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_RouteName'),5)

//Verify that there is a Route Date header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Date'),5)

//Verify that there is a Route Completed header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Completed'),5)

//Verify that the Show All Button is displayed on the page
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/btn_TableShowAllOff'),5)

//Click Show All
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table/btn_TableShowAllOff'))

//VErify Show All is Active
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/btn_TableShowAllOn'), 3)

//Check that second page is available
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageTwo_NotActive'), 3)

//Verify That there is a Route item
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'),5)

//Click on the first Route Item
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'))

String routeOneDate = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R1_Date'))

//Verify that the edit and remove buttons are present on the page
WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Details/View_Mode/btn_EditRoute'),5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/btn_DeleteRoute'),5)

//Click the Edit Route button
WebUI.click(findTestObject('Routes_OR/Route_Details/View_Mode/btn_EditRoute'))

//Verify that there are entry fields displayed
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_RouteNameActive'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_DatePickerActive'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/detCompleteSwitchActive'), 3)

//Verify that the cancel and save changes buttons are displayed on the page
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/btn_SaveChanges'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/btn_DiscardChanges'), 3)

//verify that the completed toggle button is displayed on the page
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/detCompleteSwitchActive'),3)

//click on the completed toggle button
WebUI.click(findTestObject('Routes_OR/Route_Details/Edit_Mode/detCompletedSwitchDiv'))

//Click save changes
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/btn_SaveChanges'))

//Verify that Edit Mode is not active (Input fields are back to text labels)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_RouteNameInactive'),5)	//Route Name
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_Region'),5)	//Region
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_CompleteSwitchInactive'),5)	//Completed
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_DateInactive'),5)	//Route Date

//Change to page three
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageThree_NotActive'))

//Check for a 3rd table item
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R4_Date'), 3)

//Verify 3rd table element has same date as stored before
WebUI.verifyElementText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R4_Date'), routeOneDate)

//Verify 3rd table element has Completed Status
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R4C3_Complete'), 3)