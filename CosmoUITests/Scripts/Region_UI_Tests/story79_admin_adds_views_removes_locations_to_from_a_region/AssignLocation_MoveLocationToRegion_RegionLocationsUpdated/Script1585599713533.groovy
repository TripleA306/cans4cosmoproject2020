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

/*
 * Test confirms that Locations can be assigned to a Region and the changes are saved.
 * 
 * Test will edit the fixture Region, move an unassigned locations to it, and
 * save the changes. It will then confirm the location is now assigned
 * in the LEFT table, and switch to a different region and the switch back
 * to confirm the locations are still present.
 */


//Click to Region page
WebUI.click(findTestObject('Object Repository/Regions_OR/Navigation_Bar/btnRegions'))

//Confirm Unassigned location elements are present
//Check for "Unassigned Locations" header
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/header_UnassignedLocations'), 3)

//Check that Page One is selected in the pagination
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Pagination/pagination_unassignedPageOneSelected'), 3)

//Check for first and eighth element in the Unassigned Table (Confirms minimum 8 elements exist)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR1_Street'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR8_Street'), 3)

//Click to Page Two of Regions
WebUI.click(findTestObject('Object Repository/Regions_OR/Region_List/btnPage2'))

//Click on "The New World" row (5th row)
WebUI.click(findTestObject('Object Repository/Admin_Regions/RegionTable/td_Item5'))

//Click Edit Region to enable Add/Remove
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_EditRegion'))

//Click Row 1 Location in Unassigned Table
WebUI.click(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR1_Street'))

String unassignedRowOneStreet = WebUI.getText(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR1_Street'))

//Click Add arrow button
WebUI.click(findTestObject('Object Repository/Admin_Regions/Location Tables/Assign and Remove Buttons/button_assignLocBtnENABLED'))

//Confirm Locations appeared in Left table
//Get street text of first row
String assignedRowOneStreet = WebUI.getText(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR1_Street'))

//confirm new Row One street matches street that was assigned
WebUI.verifyMatch(assignedRowOneStreet, unassignedRowOneStreet, false)

//Confirm Unassigned Row One is no longer the same Street
WebUI.verifyNotMatch(assignedRowOneStreet, WebUI.getText(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR1_Street')), false)

//Click to second Region Location page
WebUI.click(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Pagination/pagination_assignedPageTwoNotSelected'))

//Confirm three Locations are on second Region Location page (previously was two)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR1_Street'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR2_Street'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR3_Street'),3)
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR4_Street'), 3)

//Click to different Region
WebUI.click(findTestObject('Object Repository/Admin_Regions/RegionTable/td_Item4'))

//Click back to The New World
WebUI.click(findTestObject('Object Repository/Admin_Regions/RegionTable/td_Item5'))

//Verify Location still in spot and matches 
WebUI.verifyMatch(WebUI.getText(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR1_Street')), assignedRowOneStreet, false)