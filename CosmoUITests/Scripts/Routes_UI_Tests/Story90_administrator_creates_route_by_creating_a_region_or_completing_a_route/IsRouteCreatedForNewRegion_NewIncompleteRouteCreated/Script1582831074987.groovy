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
import java.sql.Time as Time
import java.text.SimpleDateFormat as SimpleDateFormat
import java.time.LocalDate as LocalDate
import java.time.format.DateTimeFormatter as DateTimeFormatter
import java.util.Date as Date
import com.kms.katalon.core.logging.KeywordLogger as KeywordLogger
import java.text.ParsePosition as ParsePosition

/*
 * Test checks if a new Route is added to the Routes Table after a new Region is added to the Database
 * Test will create a new Region on the Region page, navigate to the Routes page, confirm that the
 * new Route is present on the table, and confirm that the details pane is populated with the correct
 * region information
*/
//Assigning the desired Name and Frequency to variables 
String regionName = 'Candyland'

String regionFrequency = '10'

//Creating a LocalDate object to represent the desired Region Date
Date regionFirstDate = new Date().plus(1035)

Date regionNextDate = regionFirstDate

//Creating date strings variables to use when create the Region
SimpleDateFormat sdf = new SimpleDateFormat('MMMMM dd, yyyy')

String sFirstDate = sdf.format(regionFirstDate)

String sNextDate = sdf.format(regionNextDate)

SimpleDateFormat sdfBack = new SimpleDateFormat('YYYY-MM-dd')

String sEnterDate = sdfBack.format(regionFirstDate)

//String variable representing the final format of the generated route's name
String routeName = (regionName + ' - ') + sNextDate

//Navigate to Routes Page
WebUI.click(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'))

//Confirm new route does not exist (name check or count check?)
WebUI.verifyTextNotPresent(routeName, false)

//Navigate to Regions page
WebUI.click(findTestObject('Object Repository/Regions_OR/Navigation_Bar/btnRegions'))

//Click Add Region button
WebUI.click(findTestObject('Object Repository/Regions_OR/Region_List/btnAddRegion'))

//Fill in Region Name
WebUI.setText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_RegionName'), regionName)

//Select Region Date
WebUI.setText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_FirstCollection'), sEnterDate)

//Set Pick Up Frequency
WebUI.setText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_Frequency'), regionFrequency)

//Click Save
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_SaveChanges'))

//Verify a new Region record was made for region
WebUI.verifyElementPresent(findTestObject('Object Repository/Regions_OR/Region_List/liCandyland'), 2)

Thread.sleep(1500)

//Navigate to Routes page
WebUI.click(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'))

WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageTwo_NotActive'))

//Confirm a new Route is added (check last item in Routes table, should be 4th item)
WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Table/RouteName/table_R4_RouteName'), 2)

//Verify Route name
WebUI.verifyMatch(WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R4_RouteName')), 
    routeName, false)

//Verify Route Date
WebUI.verifyMatch(WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R4_Date')), sNextDate, 
    false)

//Verify Completed is red circle
WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Table/CompletedItems/table_R4C3_Incomplete'), 2)

//Click Route
WebUI.click(findTestObject('Routes_OR/Route_Table/RouteName/table_R4_RouteName'))

//Verify Details name is correct
WebUI.verifyMatch(WebUI.getAttribute(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_RouteNameInactive'), 
        'value'), routeName, false)

//Verify Region Name is correct against RegionName variable
WebUI.verifyMatch(WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_Region')), regionName, 
    false)

//Verify Switch is Off
WebUI.verifyElementNotChecked(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_CompleteSwitchInactive'), 
    2)

//Verify Date is correct
//Parse details pane Date value into date string of "MMM dd, yyyy" format
String detDate = WebUI.getAttribute(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_DateInactive'), 
    'value')

String routeDetDate = sdfBack.format(regionNextDate)

//Check that region's next date is the same as details view date
WebUI.verifyMatch(routeDetDate, detDate, false)

