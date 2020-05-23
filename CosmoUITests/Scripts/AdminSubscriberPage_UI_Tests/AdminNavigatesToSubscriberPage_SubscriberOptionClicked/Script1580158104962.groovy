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
 * This tests the functionality of navigating to the Subscriber Page, the admin will be able to view the sub Table
 * Authors: Jessie Smith cst231, Aaron Atkinson cst201
 */

//verify subscriber option on the nav bar exists
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'), 5)

//click the subscriber option on the nav bar
WebUI.click(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'))

//Verify that the subscriber table is present
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Subscriber_Table/tbl_Subscriber_Table'), 5)

//Verify that the subscriber Header is present
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Subscriber_Table/h1_Subcriber_Header'), 5)

