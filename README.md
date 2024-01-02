# Mits

Mits: **M**aui **I**mage **T**ools

Migrate your Xamarin.Forms apps image assets to MAUI app:

```
mits --tool migrate --source /path/to/xamarin/forms/project/folder --destination /path/to/maui/project/folder
```
Ensure that existing image assets comply with [naming restrictions](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/image?view=net-maui-8.0#load-a-local-image).

```
mits --tool rename --source /path/to/maui/project/folder
```

Find image references in XAML and C# and repair the reference to ensure it complies with [naming restrictions](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/image?view=net-maui-8.0#load-a-local-image).

```
mits --tool repair --source /path/to/maui/project/folder
```

Preview the changes a specific tool may make by adding the `--dry-run` flag:

```
mits --dry-run --tool repair --source /path/to/maui/project/folder
```

----------------------------



----------------------------

### Usage:

```
Mits | Maui Image Tools

Options:
--help
Displays the help text for Mits.

--tool
Specifies the tool to run.

--overwrite
Specifies that the tool should overwrite any destination files and not skip them.

--source
Specifies the source folder or project file for the given tool.

--destination
Specifies the destination folder or project file that the given tool should export to.

--rule-set
Specifies the fully qualified path to the rule-set json file.

--excluded
Specifies the fully qualified path for the plain text file containing newline seperated image asset names to ignore.

--dry-run
Specifies that this run of MITS should only report the changes it would make and not apply them.

--numeric-suffix-behaviour
When an image file name ends with a number value, specifies the behaviour to repair it.

'append': Appends '_n' to the end of the image name (Default).
'to-word': Converts the number to a word representation.

--numeric-prefix-behaviour
When an image file name starts with a number value, specifies the behaviour to repair it.

'append': Prepends 'n_' to the start of the image name (Default).
'to-word': Converts the number to a word representation.


----------------------------------------------------------------------------
Tools:
repair
Locates all MAUI projects within the given --source folder path, scans for image references within XAML and C# files and then converts those image references into a MAUI compliant image name.

migrate
Takes the Xamarin.iOS and Xamarin.Android projects within the given --source path (this can be either a folder or a csproj), collects their image assets and then copies them into the 'Resources/Images' folder of any MAUI projects in the --destination folder path or csproj.

rename
Locates all MAUI projects within the given --source folder path, collects their image assets and then renames them to be compliant with the MAUI image naming restrictions.```
