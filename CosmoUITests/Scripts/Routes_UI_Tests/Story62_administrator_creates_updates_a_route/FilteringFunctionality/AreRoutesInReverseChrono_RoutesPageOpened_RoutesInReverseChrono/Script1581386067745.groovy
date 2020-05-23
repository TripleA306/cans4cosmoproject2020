import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject

import com.kms.katalon.core.annotation.Keyword
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


//Tests that the Show All button is displayed above the Routes Table upon page load

//Verify current Page is 1
WebUI.verifyElementText(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageOne_Active'), "1")

//Verify that there is a Route Name header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_RouteName'),5)

//Verify that there is a Route Date header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Date'),5)

//Verify that there is a Route Completed header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Completed'),5)

//Verify That there is a Route item
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'), 5)


//Verify that the Show All Button is displayed on the page
WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Table/btn_TableShowAllOff'),5)

//Creating DateFormmater to parse Route Dates and compare them
SimpleDateFormat sdf = new SimpleDateFormat("MMMM dd, yyyy")

//Gettting Date Strings to parse into Date objects
String strDate1 = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R1_Date'))
String strDate2 = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R2_Date'))
String strDate3 = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R3_Date'))
String strDate4 = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Table/RouteDates/table_R4_Date'))

//Using Date Formatter to parse date strings pulled from table into Date objects
Date date1 = sdf.parse(strDate1)
Date date2 = sdf.parse(strDate2)
Date date3 = sdf.parse(strDate3)
Date date4 = sdf.parse(strDate4)

//Verifying that "lower" dates have a greater value than "higher" dates
WebUI.verifyEqual(date2.compareTo(date1), 0)
WebUI.verifyEqual(date3.compareTo(date2), 1)
WebUI.verifyEqual(date4.compareTo(date3), 1)


