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

//This test will ensure that paging fucntionality for the subscriber collection list functions and displays new collections for multiple pages

//click hamburger menu button


//click the collection menu item


//verify that the URL matches the expected url for the collections page.
WebUI.verifyMatch(WebUI.getUrl(), 'http://localhost:8080/#/home/history', false)

//Ensure that the first page shows the correct rows
//verify row 1 is present
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow1'), 5)
//verify row 2 is present
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow2'), 5)
//verify row 3 is present
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow3'), 5)

//Click the second page of the table
WebUI.click(findTestObject('Object Repository/CollectionsPage_OR/btnPage2'))

//verify that the new rows are displayed and that the old rows are not displayed
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page2DescendingRow1'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/CollectionsPage_OR/Page2DescendingRow2'), 5)

//verify that the old rows are not displayed
//verify row 1 is not present
WebUI.verifyElementNotPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow1'), 5)
//verify row 2 is not present
WebUI.verifyElementNotPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow2'), 5)
//verify row 3 is not present
WebUI.verifyElementNotPresent(findTestObject('Object Repository/CollectionsPage_OR/Page1DescendingRow3'), 5)

//Go back to page 1
WebUI.click(findTestObject('Object Repository/CollectionsPage_OR/btnPage1'))