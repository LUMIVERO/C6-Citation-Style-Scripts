# Field "Year" of the parent reference is empty

This programmed condition checks whether the field "Year" and the resolved year ("YearResolved") of the parent reference are empty.

## Prerequisites
Citavi 6 (or higher)

## Examples
You cite a contribution in conference proceedings or in an edited book. The year is inherited from the parent reference ("Year derived"). If the parent reference does not contain a year, a placeholder should be displayed instead of an empty output – e.g. the IEEE Reference Guide requires "(n.d.)" in this case:

- P. Contribution, "Contribution of the Proceeding," in Proceeding: Main, A. Elias, Ed., (n.d.).

To achieve this, duplicate the template of the reference type, assign this condition to the duplicate, and replace the "Year derived" component in the duplicate with a static text element containing "(n.d.)".

## Installation
See Citavi manual: [Creating Custom Templates](http://www.citavi.com/creating_custom_templates)

## Author

* **Britta Thielmann** [bthielmann](https://github.com/bthielmann)
