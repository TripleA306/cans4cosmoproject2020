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

//click the page 2 button
WebUI.click(findTestObject('Object Repository/Page_Admin - Subscribers/Paging/btn_Table_Page_2'))

//Verify that the susbcribers on page 2 are present
//first subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Paging/row_Sub_1_Page_2'), 5)

//second subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Paging/row_Sub_2_Page_2'), 5)

//Third subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Paging/row_Sub_3_Page_2'), 5)

//fourth subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Paging/row_Sub_4_Page_2'), 5)

//fifth subscriber
WebUI.verifyElementPresent(findTestObject('Object Repository/Page_Admin - Subscribers/Paging/row_Sub_5_Page_2'), 5)

//verify that the subscriebrs on the first page are not present
//Verify that the table is populated //use not visible
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


