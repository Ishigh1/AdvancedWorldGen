import string

from commands import *

file = open("Data.json", "r", encoding="utf8")
options = json.load(file)
file.close()

while True:
    output = input("command : ")
    data = output.split(" ")
    command = data[0]
    if data.__len__() > 1:
        data = data[1]
    else:
        data = ""

    if command == "exit":
        break

    elif command == "add":
        option = {}
        displayed_name = input("displayed name : ")
        option["displayed_name"] = string.capwords(displayed_name)

        internal_name = input("internal name(empty for default) : ")
        if internal_name == "":
            internal_name = option["displayed_name"].replace(" ", "")

        option["description"] = input("description : ")
        option["conflicts"] = {}
        option["children"] = {}

        conflict = input("conflict : ")
        while conflict != "":
            conflict = string.capwords(conflict).replace(" ", "")
            conflict_description = input("conflict description : ")
            option["conflicts"][conflict] = conflict_description
            conflict = input("conflict : ")
        option["hidden"] = input("hidden : ") == "y"

        parents = []
        known_name = internal_name
        while True:
            parent = input("parent : ")
            if parent == "":
                break
            parents.append(parent)
            known_name = parent + "." + known_name

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
            for not_found in conflicts_not_found:
                print("Conflict " + not_found + " wasn't found")
            break

        list_to_put = options
        for parent in parents:
            list_to_put = list_to_put[parent]["children"]
        list_to_put[internal_name] = option
        for conflict in option["conflicts"]:
            options[conflict]["conflicts"][known_name] = option["conflicts"][conflict]

        save_data(options)

    elif command == "translate":
        if data == "fr":
            language = "fr-FR"
        elif data == "ru":
            language = "ru-RU"
        else:
            continue

        translate_options(options, options, language)

    elif command == "make":
        make_files()

    elif command == "setup":
        setup()

    elif command == "conflict":
        option1 = input("option 1 : ")
        option2 = input("option 2 : ")
        conflict_description = input("description : ")

        option1 = string.capwords(option1).replace(" ", "")
        option2 = string.capwords(option2).replace(" ", "")
        options[option1]["conflicts"][option2] = conflict_description
        options[option2]["conflicts"][option1] = conflict_description

        save_data(options)

    else:
        print("command unknown")
