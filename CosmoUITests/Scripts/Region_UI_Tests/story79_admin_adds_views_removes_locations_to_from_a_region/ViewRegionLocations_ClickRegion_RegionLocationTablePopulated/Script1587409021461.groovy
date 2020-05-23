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
 * Test confirms that when Region is selected,
 * the Assigned Locations table is populated with fixture Locations
 * 
 * Test will select the Fixture Region and Edit the Region
 * Assigned Locations table will be checked to ensure fixture locations
 * are loaded into it
 * 
 */

//Click to Region page
WebUI.click(findTestObject('Object Repository/Regions_OR/Navigation_Bar/btnRegions'))

//Confirm Region Location elements are present
//Check for "Regions Locations" header
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/header_RegionLocationsNOTSELECTED'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR1_NoRegionSelected'),3)

//Check that Page One is selected in the pagination
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Pagination/pagination_assignedPageOneSelected'), 3)

//Click to Page Two of Regions
WebUI.click(findTestObject('Object Repository/Regions_OR/Region_List/btnPage2'))

//Click on "The New World" row (5th row)
WebUI.click(findTestObject('Object Repository/Admin_Regions/RegionTable/td_Item5'))

//Check for first and eighth element in the Assigned Table (Confirms minimum 8 elements exist)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR1_Street'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR8_Street'), 3)

//Click to second page
WebUI.click(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Pagination/pagination_assignedPageTwoNotSelected'))

//Check that only three elements exist by checking for existing 1, 2, and not existing 3
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR1_Street'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR2_Street'),3)
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Assigned Locations/Table Data/table_AssignedR3_Street'), 3)

