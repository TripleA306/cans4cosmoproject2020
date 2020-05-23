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

/***
 * This tests the functionality of listing Subcribers in a descending order
 * Authors: Jessie Smith cst231, Aaron Atkinson cst201
 */
//verify subscriber option on the nav bar exists
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'), 5)

//click the subscriber option on the nav bar
WebUI.click(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'))

//Verify that the subscriber table is present
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Subscriber_Table/tbl_Subscriber_Table'), 5)

//Verify that the table is populated
//first subscriber
WebUI.verifyElementPresent(findTestObject('Subscriber_Table/row_Sub_1'), 5)

//second subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_2'), 5)

//Third subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_3'), 5)

//fourth subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_4'), 5)

//Fifth subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_5'), 5)

//click on the "sort in descending order" toggle button
//By default it is already Ascending, so a second click is required
WebUI.click(findTestObject('Object Repository/Page_Admin - Subscribers/Sorting/btn_Email_Sort'))

WebUI.click(findTestObject('Object Repository/Page_Admin - Subscribers/Sorting/btn_Email_Sort'))

//Check to see if the Original values are not visible, but also confirming the new values are present
/////Checking first values arent visible
//first subscriber
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_1'), 5)

//second subscriber
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_2'), 5)

//Third subscriber
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_3'), 5)

//fourth subscriber
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_4'), 5)

//Fifth subscriber
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_5'), 5)

////////////////
//Checking for new values are present
//first subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Sorting/row_Sub_1Desc'), 5)

//second subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Sorting/row_Sub_2Desc'), 5)

//Third subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Sorting/row_Sub_3Desc'), 5)

//fourth subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Sorting/row_Sub_4Desc'), 5)

//Fifth subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Sorting/row_Sub_5Desc'), 5)

