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

//UI Test to confirm that the Administrative Routes page can be navigated to and loads
//Load browser to home page
WebUI.navigateToUrl('http://localhost:8080?noApi')

//Check if Routes button/tab exists
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'), 3)

//Click Routes button
WebUI.click(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'))

//Verify Route Header exists (confirms we're on the Routes page)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/header_RoutesTable'), 3)

//Wait for the api to respond
Thread.sleep(3000)

//Api didnt respond, Verify the Error messsage is shown
WebUI.verifyElementPresent(findTestObject('Routes_OR/Errors/text_GeneralNetworkError'), 3)

