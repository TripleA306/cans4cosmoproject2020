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
import com.kms.katalon.core.util.KeywordUtil
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import internal.GlobalVariable as GlobalVariable
import java.util.Date
import java.text.SimpleDateFormat

//Check if Routes button/tab exists
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'), 3)

//Click Routes button
WebUI.click(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'))

//Verify Route Header exists (confirms we're on the Routes page)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/header_RoutesTable'), 3)

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

//Assign first item's Route Name and Date to string variables
String tblRouteName = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'))

String tblDate = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R1_Date'))

//Click on the first Route Item
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'))

//Verify That the Additional Route Information is Displayed
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/label_RouteName'), 5 //Route Name
    )

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/label_Region'), 5 //Region
    )

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/label_Completed'), 5 //Completed
    )

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/label_Date'), 5 //Route Date
    )

//Defining Date Formatters for Details date and Table Date. Need to parse them out into Date strings to compare as formats differ
SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd")
SimpleDateFormat tblSDF = new SimpleDateFormat("MMMM dd, yyyy")


//Confirm Details RouteName, Date, and Completed is correct for RouteOne
WebUI.verifyElementAttributeValue(findTestObject('Routes_OR/Route_Details/View_Mode/details_RouteNameInactive'), "value", tblRouteName, 3)

//Get the text from Details date picker
String detailsDateString = WebUI.getAttribute(findTestObject('Routes_OR/Route_Details/View_Mode/details_DateInactive'), "value")
Date detParseDate = sdf.parse(detailsDateString)	//Parse details date into Date object
Date tblParseDate = tblSDF.parse(tblDate);			//Parse table date into Date object

//Compare details date and table date
WebUI.verifyMatch(detParseDate.toString(), tblParseDate.toString(), false)
//WebUI.verifyElementAttributeValue(findTestObject('Routes_OR/Route_Details/View_Mode/details_DateInactive'), "value", tblDate, 3)
WebUI.verifyElementAttributeValue(findTestObject('Routes_OR/Route_Details/View_Mode/details_CompleteSwitchInactive'), "value", "true", 3)


//Verify a second route item exists
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R2_RouteName'), 3)

//Assign second item's Route Name and Date to string variables
tblRouteName = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R2_RouteName'))

tblDate = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R2_Date'))

//Click second route Item
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R2_RouteName'))

//Confirm Details RouteName, Date, and Completed is correct for RouteTwo
WebUI.verifyElementText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R2_RouteName'), tblRouteName)

//Get the text from Details date picker
detailsDateString = WebUI.getAttribute(findTestObject('Routes_OR/Route_Details/View_Mode/details_DateInactive'), "value")
detParseDate = sdf.parse(detailsDateString)	//Parse details date into Date object
tblParseDate = tblSDF.parse(tblDate);			//Parse table date into Date object

//Compare details date and table date
WebUI.verifyMatch(detParseDate.toString(), tblParseDate.toString(), false)

WebUI.verifyElementAttributeValue(findTestObject('Routes_OR/Route_Details/View_Mode/details_CompleteSwitchInactive'), "value", "true", 3)


