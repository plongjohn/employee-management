<?php

// German UI texts. Placeholders in braces, e.g. {count}, are replaced by Translator.

return [
    'AppTitle' => 'Mitarbeiterverwaltung',
    'DateFormat' => 'd.m.Y',
    'Close' => 'Schließen',

    // Employee list
    'ListHeading' => 'Mitarbeiter',
    'SearchLabel' => 'Suche',
    'SearchPlaceholder' => 'Name oder E-Mail suchen …',
    'SearchButton' => 'Suchen',
    'FilterDepartmentLabel' => 'Abteilung',
    'FilterAllDepartments' => 'Alle Abteilungen',
    'ColumnName' => 'Name',
    'ColumnEmail' => 'E-Mail',
    'ColumnDepartment' => 'Abteilung',
    'ColumnHireDate' => 'Eintrittsdatum',
    'ColumnActions' => 'Aktionen',
    'SortBy' => 'Nach {column} sortieren',
    'EmployeeCount' => '{count} Mitarbeiter',
    'PageOf' => 'Seite {page} von {pages}',
    'PageSizeLabel' => 'Einträge pro Seite',
    'PageSizeOption' => '{size} pro Seite',
    'Pagination' => 'Seitennavigation',
    'FirstPage' => 'Erste Seite',
    'PreviousPage' => 'Vorherige Seite',
    'NextPage' => 'Nächste Seite',
    'LastPage' => 'Letzte Seite',
    'EmptyList' => 'Keine Mitarbeiter gefunden.',
    'AddEmployee' => 'Mitarbeiter hinzufügen',
    'EditEmployee' => '{name} bearbeiten',
    'DeleteEmployee' => '{name} löschen',

    // Employee form
    'FormTitleAdd' => 'Mitarbeiter hinzufügen',
    'FormTitleEdit' => 'Mitarbeiter bearbeiten',
    'LabelFirstName' => 'Vorname',
    'LabelLastName' => 'Nachname',
    'LabelEmail' => 'E-Mail',
    'LabelDepartment' => 'Abteilung',
    'LabelHireDate' => 'Eintrittsdatum',
    'RequiredField' => 'Pflichtfeld',
    'DepartmentPlaceholder' => 'Bitte wählen …',
    'Save' => 'Speichern',
    'Cancel' => 'Abbrechen',
    'SaveNoChangesHint' => 'Es gibt keine Änderungen zum Speichern.',
    'SaveRequiredFieldsHint' => 'Bitte füllen Sie zuerst alle Pflichtfelder (*) aus.',
    'DuplicateEmail' => 'Diese E-Mail-Adresse wird bereits von einem anderen Mitarbeiter verwendet.',
    'ConflictHeading' => 'Der Mitarbeiter wurde inzwischen geändert',
    'ConflictText' => 'Ein anderer Benutzer hat diesen Mitarbeiter geändert, nachdem Sie ihn geöffnet haben. '
        . 'Ihre Änderungen wurden nicht gespeichert. „Neu laden“ zeigt die aktuellen Daten an, Ihre Eingaben '
        . 'gehen dabei verloren. „Zurück“ lässt Ihre Eingaben stehen.',
    'ConflictReload' => 'Neu laden',
    'ConflictBack' => 'Zurück',
    'EmployeeNotFound' => 'Dieser Mitarbeiter wurde inzwischen gelöscht. Die Liste wurde aktualisiert.',
    'DiscardHeading' => 'Änderungen verwerfen?',
    'DiscardText' => 'Ihre Änderungen wurden noch nicht gespeichert.',
    'DiscardConfirm' => 'Verwerfen',
    'DiscardKeepEditing' => 'Weiter bearbeiten',

    // Delete
    'DeleteHeading' => 'Mitarbeiter löschen?',
    'DeleteText' => '„{name}“ wird endgültig gelöscht.',
    'DeleteConfirm' => 'Löschen',
    'DeleteConflict' => 'Dieser Mitarbeiter wurde inzwischen von einem anderen Benutzer geändert und deshalb '
        . 'nicht gelöscht. Bitte prüfen Sie die aktuellen Daten.',
    'DeleteNotFound' => 'Dieser Mitarbeiter wurde bereits gelöscht.',

    // Status messages after saving or deleting
    'StatusCreated' => 'Mitarbeiter wurde angelegt.',
    'StatusSaved' => 'Änderungen wurden gespeichert.',
    'StatusDeleted' => 'Mitarbeiter wurde gelöscht.',

    // Validation
    'ValidationFirstNameRequired' => 'Bitte geben Sie einen Vornamen ein.',
    'ValidationFirstNameTooLong' => 'Der Vorname darf höchstens {max} Zeichen lang sein.',
    'ValidationLastNameRequired' => 'Bitte geben Sie einen Nachnamen ein.',
    'ValidationLastNameTooLong' => 'Der Nachname darf höchstens {max} Zeichen lang sein.',
    'ValidationEmailRequired' => 'Bitte geben Sie eine E-Mail-Adresse ein.',
    'ValidationEmailTooLong' => 'Die E-Mail-Adresse darf höchstens {max} Zeichen lang sein.',
    'ValidationEmailInvalid' => 'Bitte geben Sie eine gültige E-Mail-Adresse ein, z. B. vorname.nachname@firma.com.',
    'ValidationDepartmentRequired' => 'Bitte wählen Sie eine Abteilung aus.',
    'ValidationDepartmentNotFound' => 'Die gewählte Abteilung existiert nicht mehr. Bitte wählen Sie eine andere aus.',
    'ValidationHireDateRequired' => 'Bitte geben Sie ein Eintrittsdatum ein.',
    'ValidationHireDateInvalid' => 'Bitte geben Sie ein gültiges Datum ein.',
    'ValidationHireDateTooEarly' => 'Das Eintrittsdatum darf nicht vor dem {date} liegen.',
    'ValidationHireDateTooFarInFuture' => 'Das Eintrittsdatum darf höchstens ein Jahr in der Zukunft liegen.',

    // Error pages
    'ErrorHeading' => 'Das hat leider nicht geklappt',
    'ErrorNotFoundHeading' => 'Seite nicht gefunden',
    'ErrorNotFound' => 'Die angeforderte Seite gibt es nicht.',
    'ErrorInvalidRequest' => 'Die Anfrage war ungültig oder Ihre Sitzung ist abgelaufen. Bitte laden Sie die '
        . 'Seite neu und versuchen Sie es erneut.',
    'ErrorConfigurationInvalid' => 'Die Datenbankverbindung ist nicht konfiguriert. Bitte prüfen Sie die '
        . 'Zugangsdaten in der Datei config/config.php (siehe README).',
    'ErrorDatabaseUnavailable' => 'Beim Zugriff auf die Datenbank ist ein Fehler aufgetreten. Bitte prüfen Sie, '
        . 'ob die Datenbank erreichbar ist, und versuchen Sie es erneut. Details stehen in der Log-Datei.',
    'ErrorUnexpected' => 'Ein unerwarteter Fehler ist aufgetreten. Details stehen in der Log-Datei.',
    'BackToList' => 'Zur Mitarbeiterliste',
];
