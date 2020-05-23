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

/**
 * This tests the functionality of deleting a Subscriber from the Subscriber list.
 * @author Jessie Smith cst231, Aaron Atkinson cst201
 *
 */
//Open the browser to the Admin Page
//Open the browser


//verify subscriber option on the nav bar exists
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'), 5)

//click the subscriber option on the nav bar
WebUI.click(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'))

//Verify that the subscriber table is present
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Subscriber_Table/tbl_Subscriber_Table'), 5)

//Verify that the table is populated
//first subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_1'), 5)

//second subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_2'), 5)

//Third subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_3'), 5)

//fourth subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_4'), 5)

//Fifth subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_5'), 5)

//Click on the remove button to set the subscriber to inactive
WebUI.click(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/btn_Delete_Subscriber_1'))

//Click the okay button to confirm the deletion of the Subscriber (set to inactive)
WebUI.click(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/modal_Delete_Ok'))

//verify that the deleted subscriber is not present in the Subscriber table
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Subscriber_Table/row_Sub_1'), 5)


