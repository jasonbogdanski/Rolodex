include "./psake-build-helpers.ps1"

properties {
    $configuration = 'Release'
    $owner = 'Jason Bogdanski'
    $product = 'Rolodex'
    $yearInitiated = '2022'
    $projectRootDirectory = "$(resolve-path .)"
    $publish = "$projectRootDirectory/publish"
    $testResults = "$projectRootDirectory/TestResults"
}
 
task default -depends Clean, Migrate-Database, Test
task CI -depends Clean, Test, Publish -description "Continuous Integration process"
task Rebuild -depends Clean, Compile -description "Rebuild the code and database, no testing"

task Info -description "Display runtime information" {
    exec { dotnet --info }
}

task Migrate-TestDatabase -alias mtd -description "Recreate the testing database" {
    exec { dotnet grate `
            -c "Server=(localdb)\mssqllocaldb;Database=Rolodex-Test;Trusted_Connection=True;MultipleActiveResultSets=True;" `
            -f "Rolodex.Web\DatabaseScripts" `
            --silent `
            --drop `
    }
}

task Test -depends Compile, Migrate-TestDatabase -description "Run unit tests" {
    # find any directory that ends in "Tests" and execute a test
    exec { dotnet test --configuration $configuration --no-build -l "trx;LogFileName=$($_.name).trx" -l "html;LogFileName=$($_.name).html" -l "console;verbosity=normal" -r $testResults }
}
 
task Compile -depends Info -description "Compile the solution" {
    exec { dotnet build --configuration $configuration --nologo -p:"Product=$($product)" -p:"Copyright=$(get-copyright)" } -workingDirectory .
}

task Publish -depends Compile -description "Publish the primary projects for distribution" {
    remove-directory-silently $publish
    exec { publish-project } -workingDirectory Rolodex.Web
}

task Migrate-Database -alias md -description "Migrate the changes into the runtime database" {
    exec { dotnet grate `
            -c "Server=(localdb)\mssqllocaldb;Database=Rolodex;Trusted_Connection=True;MultipleActiveResultSets=True;" `
            -f "Rolodex.Web\DatabaseScripts" `
            --silent `
    } 
}

task Rebuild-Database -alias rd -description "Drop and re-create the local runtime database" {
    exec { dotnet grate `
            -c "Server=(localdb)\mssqllocaldb;Database=Rolodex;Trusted_Connection=True;MultipleActiveResultSets=True;" `
            -f "Rolodex.Web\DatabaseScripts" `
            --silent `
            --drop `
    }
}
  
task Clean -description "Clean out all the binary folders" {
    exec { dotnet clean --configuration $configuration /nologo } -workingDirectory .
    remove-directory-silently $publish
    remove-directory-silently $testResults
}

task ? -alias help -description "Display help content and possible targets" {
    WriteDocumentation
}