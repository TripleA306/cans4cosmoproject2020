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

//Tests that the Show All button will 
//	Show Incomplete and Complete Routes when toggled on,
//	Hide Complete routes when toggled off
//	Updates Pagination and Route Tables items based on the number of Routes and mode


//Verify current Page is 1
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageOne_Active'), 3)

//Verify that there is a Route Name header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_RouteName'),5)

//Verify that there is a Route Date header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Date'),5)

//Verify that there is a Route Completed header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Completed'),5)

//Verify That there is a Route item
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'),5)

//Verify that the Show All Button is displayed on the page
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/btn_TableShowAllOff'),5)

//Check that 5 items are in table. All incomplete
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R1C3_Incomplete'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R2C3_Incomplete'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R3C3_Incomplete'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R4C3_Incomplete'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R5C3_Incomplete'), 3)

//Click on the Show All Button --> setting the status to true and showing all active records
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table/btn_TableShowAllOff'))

//verify that the pagination is updated after clicking the Show All Button
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageTwo_NotActive'),3)

//Click on the page 2 button
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageTwo_NotActive'))

//Check that 3rd element appeared
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R4C3_Complete'),3)

//Verify 2 Complete elements exist
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R4C3_Complete'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R5C3_Complete'), 3)

//Click on the Show All Button to toggle it off
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table/btn_TableShowAllOn'))

//verify that we are on the second page of the table
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageTwo_Active'),3)

//Verify that the 3rd, 4th and 5th row in the table is not shown when the Show All Button is toggled off
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R4C3_Complete'), 3)
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Routes_OR/Route_Table/CompletedItems/table_R5C3_Complete'), 3)






