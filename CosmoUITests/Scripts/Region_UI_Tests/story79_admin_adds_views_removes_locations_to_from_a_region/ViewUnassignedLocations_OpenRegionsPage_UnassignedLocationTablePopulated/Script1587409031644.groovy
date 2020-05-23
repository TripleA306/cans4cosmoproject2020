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
 * Test confirms that Unassigned Locations table is populated when View Locations mode is toggled ON
 * Test will select the Fixture Region, toggle View Locations mode ON, and then verify that
 * the Unassigned Locations table has the correct fixtures loaded into it
 */



//Click to Regions Page
WebUI.click(findTestObject('Object Repository/Regions_OR/Navigation_Bar/btnRegions'))

//Confirm Unassigned location elements are present
//Check for "Unassigned Locations" header
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/header_UnassignedLocations'), 3)

//Check that Page One is selected in the pagination
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Pagination/pagination_unassignedPageOneSelected'), 3)

//Check for first and eighth element in the Unassigned Table (Confirms minimum 8 elements exist)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR1_Street'), 3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR8_Street'), 3)

//Click to second page
WebUI.click(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Pagination/pagination_unassignedPageTwoNotSelected'))

//Check that only three elements exist by checking for existing 1, 3, and not existing 4
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR1_Street'),3)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR3_Street'),3)
WebUI.verifyElementNotPresent(findTestObject('Object Repository/Admin_Regions/Location Tables/Unassigned Locations/Table Data/table_UnassignedR4_Street'), 3)
