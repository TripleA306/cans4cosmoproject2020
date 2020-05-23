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

//Test confirms that the Delete Route button is displayed in the Details pane when a Route is selected from the table

//Verify current Page is 1
WebUI.verifyElementText(findTestObject('Object Repository/Routes_OR/Route_Table_Pagination/pagination_PageOne_Active'),
	'1')

//Verify that there is a Route Name header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_RouteName'), 5)

//Verify that there is a Route Date header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Date'), 5)

//Verify that there is a Route Completed header
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/tableHeader_Completed'), 5)

//Verify That there is a Route item
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'), 5)

//Click on the first Route Item
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Table/RouteName/table_R1_RouteName'))

String routeName = WebUI.getAttribute(findTestObject('Routes_OR/Route_Details/View_Mode/details_RouteNameInactive'), "value")

//Verify that the remove button is present
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/btn_DeleteRoute'), 5)

//Click on the remove Route Button
WebUI.click(findTestObject('Object Repository/Routes_OR/Route_Details/View_Mode/btn_DeleteRoute'))

//verify that the delete route modal appears
WebUI.verifyElementPresent(findTestObject('Routes_OR/Route_Details/Delete_Modal/modal_headerDeleteRoute'), 5)

//verify that the cancel and delete buttons on the modal are present
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Delete_Modal/modal_btnCancel'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Delete_Modal/modal_btnOK'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Routes_OR/Route_Details/Delete_Modal/modal_RouteName'), 5)

String modalRouteName = WebUI.getText(findTestObject('Object Repository/Routes_OR/Route_Details/Delete_Modal/modal_RouteName'))

//verify that the name of the route on the modal is correct
WebUI.verifyMatch(routeName, modalRouteName, false)







