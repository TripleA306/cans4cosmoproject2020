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
import java.util.Date as Date
import java.text.SimpleDateFormat as SimpleDateFormat
import java.time.format.DateTimeFormatter as DateTimeFormatter
import java.time.format.FormatStyle as FormatStyle
import java.time.temporal.TemporalAccessor as TemporalAccessor
import java.time.LocalDate as LocalDate
import java.time.LocalDateTime as LocalDateTime
import java.util.Locale as Locale
import com.kms.katalon.core.logging.KeywordLogger as KeywordLogger
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject

//Testing that editing and saving the Route Date is reflected in both the Details pane and the Routes table
//Verify current Page is 1
WebUI.verifyElementText(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageOne_Active'), 
    '1')

KeywordLogger log = new KeywordLogger()

DateTimeFormatter dtfOne = DateTimeFormatter.ISO_LOCAL_DATE

TemporalAccessor ta = dtfOne.parse('2100-01-01')

LocalDate dt = LocalDate.from(ta)

DateTimeFormatter dtf = DateTimeFormatter.ofLocalizedDate(FormatStyle.SHORT).withLocale(Locale.CANADA)

String enterDate = dtf.format(dt)

String preYear = enterDate.substring(0, 6)

String sDate = ((preYear + 21) + enterDate.substring(6)).replace('/', '')

//Verify that there is a Route Name header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_RouteName'), 5)

//Verify that there is a Route Date header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Date'), 5)

//Verify that there is a Route Completed header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Completed'), 5)

//Verify That there is a Route item
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'), 5)

//Go to page two
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageTwo_NotActive'))

//Click on the 3rd Route Item
WebUI.click(findTestObject('Routes_OR/Route_Table/RouteName/table_R3_RouteName'))

//Verify that the edit button is present on the page
WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Details/View_Mode/btn_EditRoute'), 5)

//Click the Edit Route button
WebUI.click(findTestObject('Routes_OR/Route_Details/View_Mode/btn_EditRoute'))

//Verify that there are entry fields displayed
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_RouteNameActive'), 
    3)

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_DatePickerActive'), 
    3)

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/detCompleteSwitchActive'), 
    3)

//Verify that the cancel and save changes buttons are displayed on the page
WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Details/Edit_Mode/btn_SaveChanges'), 3)

WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Details/Edit_Mode/btn_DiscardChanges'), 3)

WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_DatePickerActive'))

WebUI.sendKeys(findTestObject(null), sDate)

//Enter in new date value for the Date Picker
//WebUI.modifyObjectProperty(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/input_DatePickerActive'), "value", "equals", "01012100", false)
//Click the Save Changes button
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Details/Edit_Mode/btn_SaveChanges'))

Thread.sleep(2000)

//Verify that Edit Mode is not active (Input fields are back to text labels)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_RouteNameInactive'), 
    5 //Route Name
    )

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_Region'), 5 //Region
    )

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_CompleteSwitchInactive'), 
    5 //Completed
    )

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/details_DateInactive'), 5 //Route Date
    )

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/btn_EditRoute'), 3)

WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/btn_DeleteRoute'), 3)

//Defining Date Formatters for Details date and Table Date. Need to parse them out into Date strings to compare as formats differ
SimpleDateFormat sdf = new SimpleDateFormat('yyyy-MM-dd')

SimpleDateFormat tblSDF = new SimpleDateFormat('MMM dd, yyyy')

String tblDate = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R3_Date'))

String detailsDateString = WebUI.getAttribute(findTestObject('Routes_OR/Route_Details/View_Mode/details_DateInactive'), 
    'value')

Date detParseDate = sdf.parse(detailsDateString //Parse details date into Date object
    )

Date tblParseDate = tblSDF.parse(tblDate //Parse table date into Date object
    )

//Verify P1R1C1 Route Date is the same as Details Route Date
WebUI.verifyMatch(detParseDate.toString(), tblParseDate.toString(), false)

