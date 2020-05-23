import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import internal.GlobalVariable as GlobalVariable

//this test will ensure that the export to csv button is diabled when a route with no locations is selected.
//Check if Routes button/tab exists
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'), 3)

//Click Routes button
WebUI.click(findTestObject('Object Repository/Routes_OR/Navigation_Bar/button_NavRoute'))

//Verify Route Header exists (confirms we're on the Routes page)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/header_RoutesTable'), 3)

//select the route "Logans Loop" for March 17, 2020
WebUI.click(findTestObject('Routes_OR/Route_Table/RouteName/RowNathansCircuit'))

//verify the button can not be clicked
//WebUI.verifyElementNotClickable(findTestObject('Object Repository/Routes_OR/Route_Details/btnExport'))
//verify error toast appears
WebUI.verifyTextPresent('This region contains no locations', false)

