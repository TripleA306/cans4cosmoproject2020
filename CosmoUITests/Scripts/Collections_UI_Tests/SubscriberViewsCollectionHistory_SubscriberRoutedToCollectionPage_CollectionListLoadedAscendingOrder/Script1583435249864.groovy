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

//This test will ensure that a subscriber can view the collection list in ascending order
WebUI.verifyMatch(WebUI.getUrl(), 'http://localhost:8080/#/home/history', false)

//Ensure that the first page shows the correct rows
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow1'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow2'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow3'), 5)

//Click the sort button to get to ascending
WebUI.click(findTestObject('Object Repository/CollectionsPage_OR/sortBy'))

//Verify that the sort is displaying ascending order
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1AscendingRow1'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1AscendingRow2'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1AscendingRow3'), 5)

//ensure that the descending rows are no longer showing
WebUI.verifyElementNotPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow1'), 5)
WebUI.verifyElementNotPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow2'), 5)


//Click the sort buttonto get back to descending order
WebUI.click(findTestObject('Object Repository/CollectionsPage_OR/sortBy'))