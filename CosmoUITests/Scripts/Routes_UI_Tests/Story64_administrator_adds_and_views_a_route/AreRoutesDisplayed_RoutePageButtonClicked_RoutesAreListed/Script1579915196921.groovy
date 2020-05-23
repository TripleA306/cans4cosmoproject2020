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

//Check if Routes button/tab exists
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'),3)

//Click Routes button
WebUI.click(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'))

//Verify Route Header exists (confirms we're on the Routes page)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/header_RoutesTable'), 3)

//Verify that Pagination bar appears
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageOne_Active'),5)

//Verify current Page is 1
WebUI.verifyElementText(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageOne_Active'), "1")

//Verify that there is a Route Name header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_RouteName'),5)

//Verify that there is a Route Date header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Date'),5)

//Verify that there is a Route Completed header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Completed'),5)

//Verify That there is a Route item
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'),5)

//Verify that there is a second Route item 
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R2_RouteName'), 5)

//Verify Details Component loaded - Check for Details Header and Select Route text
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/header_Details'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/text_SelectRoute'),3)
