import json


def make(language):
    file = open("Data.json", "r")
    options = json.load(file)
    file.close()

    translation = {}
    base = {"Mods": {"AdvancedWorldGen": translation}}

    translation["Conflict"] = {}
    conflicts = translation["Conflict"]
    for key in options:
        option = options[key]

        name = option["displayed_name"]
        if type(name) is dict:
            name = name[language]

        description = option["description"]
        if type(description) is dict:
            description = description[language]

        translation[option["internal_name"]] = {"$parentVal": name,
                                                "Description": description}
        conflicts_dict = option["conflicts"]
        if conflicts_dict is not None:
            internal_conflicts = {}
            conflicts[option["internal_name"]] = internal_conflicts
            for conflict_key in conflicts_dict:
                description = conflicts_dict[conflict_key]
                if type(description) is dict:
                    description = description[language]
                internal_conflicts[conflict_key] = description

    file = open(language + ".json", "w")
    json.dump(base, file, indent=4, sort_keys=True)
    file.close()

    json_text = {}
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
        json_text[key] = o

    file = open("Options.json", "w")
    json.dump(json_text, file, indent=4)
    file.close()


def translate(initial, language):
    if type(initial) is not dict:
        result = {"en-US": initial}
    elif initial.__contains__(language):
        return initial
    else:
        result = initial

    print("english value : " + result["en-US"])
    result[language] = input("translation : ")

    return result
