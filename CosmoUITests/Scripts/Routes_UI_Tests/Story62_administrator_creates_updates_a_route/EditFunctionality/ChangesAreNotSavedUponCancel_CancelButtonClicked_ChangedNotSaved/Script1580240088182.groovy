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

//Testing that changes do not affect underlying Route information if changes are made but not saved/are cancelled

//Verify current Page is 1
WebUI.verifyElementText(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageOne_Active'), 
    '1')

//Verify that there is a Route Name header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_RouteName'), 5)

//Verify that there is a Route Date header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Date'), 5)

//Verify that there is a Route Completed header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Completed'), 5)

//Verify That there is a Route item
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'), 5)

//Click on the first Route Item
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'))


//Verify that the edit button is present on the page
WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Details/View_Mode/btn_EditRoute'), 5)

//Click the Edit Route button
WebUI.click(findTestObject('Routes_OR/Route_Details/View_Mode/btn_EditRoute'))


//Verify that there are entry fields displayed
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_RouteNameActive'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_DatePickerActive'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/detCompleteSwitchActive'),3)

//Verify that the cancel and save changes buttons are displayed on the page
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/btn_SaveChanges'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/btn_DiscardChanges'), 3)

//Setting variable for name to change to
String changedName = 'Test Changed Name'

String oldName = WebUI.getAttribute(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_RouteNameActive'), "value")

//Enter a new name that is too short into the RouteName text field
WebUI.modifyObjectProperty(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_RouteNameActive'), "value", "equals", changedName, false)

//Click cancel changes
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/btn_DiscardChanges'))

//Verify that Edit Mode is not active (Input fields are back to text labels)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_RouteNameInactive'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_Region'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_DateInactive'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_CompleteSwitchInactive'), 3)

//Verify that Table Name and details name match each other and the changed name
//Table Name check
WebUI.verifyMatch(WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName')), oldName, false)

//Details pane check
WebUI.verifyMatch(WebUI.getAttribute(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_RouteNameInactive'),"value"), oldName, false)

