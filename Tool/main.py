import collections
import json
import shutil
import string

file = open("Data.json", "r")
options = json.load(file)
file.close()

while True:
    command = input("command : ")
    if command == "exit":
        break

    elif command == "add":
        option = {}
        option["displayed_name"] = input("displayed name : ")
        option["displayed_name"] = string.capwords(option["displayed_name"])
        option["internal_name"] = input("internal name(empty for default) : ")
        if option["internal_name"] == "":
            option["internal_name"] = option["displayed_name"].replace(" ", "")
        option["description"] = input("description : ")
        option["conflicts"] = {}
        conflict = input("conflict : ")
        while conflict != "":
            conflict = string.capwords(conflict).replace(" ", "")
            conflict_description = input("conflict description : ")
            option["conflicts"][conflict] = conflict_description
            conflict = input("conflict : ")
        option["hidden"] = input("hidden : ") == "y"

        if option["internal_name"] in options:
            print("option already exists")
        conflicts_not_found = option["conflicts"].copy()
        for o in options:
            if o == "None":
                continue
            if options[o]["displayed_name"] == option["displayed_name"]:
                print("Displayed name already exists")
                break
            if options[o]["description"] == option["description"]:
                print("Description already exists")
                break
            if o in conflicts_not_found:
                conflicts_not_found.pop(o)
        if len(conflicts_not_found) != 0:
            print("A conflict wasn't found")
            break

        options[option["internal_name"]] = option
        for conflict in option["conflicts"]:
            options[conflict]["conflicts"][option["internal_name"]] = option["conflicts"][conflict]

        file = open("Data.json", "w")
        json.dump(options, file, indent=4, sort_keys=True)
        file.close()

    elif command == "make":
        file = open("Data.json", "r")
        options = json.load(file)
        file.close()

        base = open("base_en-US.json", "r")
        base_translation = json.load(base)
        translation = base_translation["Mods"]["AdvancedWorldGen"]
        base.close()

        conflicts = translation["Conflict"]
        for key in options:
            option = options[key]
            option_translation = {}
            translation[option["internal_name"]] = option_translation
            option_translation["$parentVal"] = option["displayed_name"]
            option_translation["Description"] = option["description"]
            conflicts[option["internal_name"]] = option["conflicts"]

        file = open("en-US.json", "w")
        json.dump(base_translation, file, indent=4, sort_keys=True)
        file.close()

        jsonText = {}
        current_id = 1
        current_hidden_id = 101
        for key in options:
            option = options[key]
            o = {}
            if option["hidden"]:
                o["Hidden"] = True
                o["id"] = current_hidden_id
                current_hidden_id += 1
            else:
                o["Hidden"] = False
                o["id"] = current_id
                current_id += 1
            o["Conflicts"] = list(option["conflicts"].keys())
            jsonText[key] = o

        file = open("Options.json", "w")
        json.dump(jsonText, file, indent=4)
        file.close()

    elif command == "setup":
        shutil.copy("en-US.json", "../Localization/en-US.hjson")
        shutil.copy("Options.json", "../Options.json")

    elif command == "conflict":
        option1 = input("option 1 : ")
        option2 = input("option 2 : ")
        conflict_description = input("description : ")

        option1 = string.capwords(option1).replace(" ", "")
        option2 = string.capwords(option2).replace(" ", "")
        options[option1]["conflicts"][option2] = conflict_description
        options[option2]["conflicts"][option1] = conflict_description

        file = open("Data.json", "w")
        json.dump(options, file, indent=4, sort_keys=True)
        file.close()

    else:
        print("command unknown")