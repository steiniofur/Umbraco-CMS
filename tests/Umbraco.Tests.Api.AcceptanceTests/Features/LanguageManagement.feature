Feature: Manage languages in the system

    @createLanguageItem
    Scenario: Language gets created successfully
        When I create a new language with the following details
            | name   | isoCode | IsDefault | IsMandatory
            | Danish | da      | false     | false
        Then The language should be created successfully

    Scenario: Language gets deleted successfully
        Given I have a language with the following details
            | name   | isoCode | IsDefault | IsMandatory
            | Danish | da      | false     | false
        When that language gets deleted
        Then The language should be deleted successfully
