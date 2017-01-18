using System.Collections.Generic;

namespace WorldQuestSettings.WorldQuestSettings.GroupFinder
{
    internal class QuestLists
    {
        public static HashSet<uint> BlackListed = new HashSet<uint>
        {
            43943, // Withered Army Training
            42725, // Sharing the Wealth
            42880, // Meeting their Quota
            42178, // Shock Absorber
            42173, // Electrosnack  
            44011, // Lost Wisp
            43774, // Ley Race
            43764, // Ley Race
            43753, // Ley Race
            43325, // Ley Race
            43769, // Ley Race
            43772, // Enigmatic
            43767, // Enigmatic
            43756, // Enigmatic
            45032, // Like the Wind
            45046, // Like the Wind
            45047, // Like the Wind
            45048, // Like the Wind
            45049, // Like the Wind
            45068, // Barrels o' fun
            45069, // Barrels o' fun
            45070, // Barrels o' fun
            45071, // Barrels o' fun
            45072, // Barrels o' fun
            44786, // Midterm: Rune Aptitude
            41327, // Supplies Needed: Stormscales
            41345, // Supplies Needed: Stormscales
            41318, // Supplies Needed: Felslate
            41237, // Supplies Needed: Stonehide Leather
            41339, // Supplies Needed: Stonehide Leather
            41351, // Supplies Needed: Stonehide Leather
            41207, // Supplies Needed: Leystone
            41298, // Supplies Needed: Fjarnskaggl
            41315, // Supplies Needed: Leystone
            41316, // Supplies Needed: Leystone
            41317, // Supplies Needed: Leystone
            41303, // Supplies Needed: Starlight Roses
            41288, // Supplies Needed: Aethril
        };

        public static HashSet<uint> RaidQuests = new HashSet<uint>
        {
            42820, // DANGER: Aegir Wavecrusher
            41685, // DANGER: Ala'washte
            44113, // DANGER: Anachronos
            43091, // DANGER: Arcanor Prime
            44118, // DANGER: Auditor Esiel
            44121, // DANGER: Az'jatar
            44189, // DANGER: Bestrix
            42861, // DANGER: Boulderfall, the Eroded
            42864, // DANGER: Captain Dargun
            43121, // DANGER: Chief Treasurer Jabrill
            41697, // DANGER: Colerian, Alteria, and Selenyi
            43175, // DANGER: Deepclaw
            41695, // DANGER: Defilia
            42785, // DANGER: Den Mother Ylva
            41093, // DANGER: Durguth
            43346, // DANGER: Ealdis
            43059, // DANGER: Fjordun
            42806, // DANGER: Fjorlag, the Grave's Chill
            43345, // DANGER: Harbinger of Screams
            43079, // DANGER: Immolian
            44190, // DANGER: Jade Darkhaven
            44191, // DANGER: Karthax
            43798, // DANGER: Kosumoth the Hungering
            42964, // DANGER: Lagertha
            44192, // DANGER: Lysanis Shadesoul
            43152, // DANGER: Lytheron
            44114, // DANGER: Magistrix Vilessa
            42927, // DANGER: Malisandra
            43098, // DANGER: Marblub the Massive
            41696, // DANGER: Mawat'aki
            43027, // DANGER: Mortiferous
            43333, // DANGER: Nylaathria the Forgotten
            41703, // DANGER: Ormagrogg
            41816, // DANGER: Oubdob da Smasher
            43347, // DANGER: Rabxach
            42963, // DANGER: Rulf Bonesnapper
            42991, // DANGER: Runeseer Sigvid
            42797, // DANGER: Scythemaster Cil'raman
            44193, // DANGER: Sea King Tidross
            41700, // DANGER: Shalas'aman
            44122, // DANGER: Sorallus
            42953, // DANGER: Soulbinder Halldora
            43072, // DANGER: The Whisperer
            44194, // DANGER: Torrentius
            43040, // DANGER: Valakar the Thirsty
            44119, // DANGER: Volshax, Breaker of Will
            43101, // DANGER: Withdoctor Grgl-Brgl
            41779, // DANGER: Xavrix
            44017, // WANTED: Apothecary Faldren
            44032, // WANTED: Apothecary Faldren
            42636, // WANTED: Arcanist Shal'iman
            43605, // WANTED: Arcanist Shal'iman
            42620, // WANTED: Arcavellus
            43606, // WANTED: Arcavellus
            41824, // WANTED: Arru
            44289, // WANTED: Arru
            44301, // WANTED: Bahagar
            44305, // WANTED: Bahagar
            41836, // WANTED: Bodash the Hoarder
            43616, // WANTED: Bodash the Hoarder
            41828, // WANTED: Bristlemaul
            44290, // WANTED: Bristlemaul
            43426, // WANTED: Brogozog
            43607, // WANTED: Brogozog
            42796, // WANTED: Broodmother Shu'malis
            44016, // WANTED: Cadraeus
            44031, // WANTED: Cadraeus
            43430, // WANTED: Captain Volo'ren
            43608, // WANTED: Captain Volo'ren
            41826, // WANTED: Crawshuk the Hungry
            44291, // WANTED: Crawshuk the Hungry
            44299, // WANTED: Darkshade
            44304, // WANTED: Darkshade
            43455, // WANTED: Devouring Darkness
            43617, // WANTED: Devouring Darkness
            43428, // WANTED: Doomlord Kazrok
            43609, // WANTED: Doomlord Kazrok
            44298, // WANTED: Dreadbog
            44303, // WANTED: Dreadbog
            43454, // WANTED: Egyl the Enduring
            43620, // WANTED: Egyl the Enduring
            43434, // WANTED: Fathnyr
            43621, // WANTED: Fathnyr
            43436, // WANTED: Glimar Ironfist
            43622, // WANTED: Glimar Ironfist
            44030, // WANTED: Guardian Thor'el
            44013, // WANTED: Guardian Thor'el
            41819, // WANTED: Gurbog da Basher
            43618, // WANTED: Gurbog da Basher
            43453, // WANTED: Hannval the Butcher
            43623, // WANTED: Hannval the Butcher
            44021, // WANTED: Hertha Grimdottir
            44029, // WANTED: Hertha Grimdottir
            43427, // WANTED: Infernal Lord
            43610, // WANTED: Infernal Lord
            43611, // WANTED: Inquisitor Tivos
            42631, // WANTED: Inquisitor Tivos
            43452, // WANTED: Isel the Hammer
            43624, // WANTED: Isel the Hammer
            43460, // WANTED: Kiranys Duskwhisper
            43629, // WANTED: Kiranys Duskwhisper
            44028, // WANTED: Lieutenant Strathmar
            44019, // WANTED: Lieutenant Strathmar
            44018, // WANTED: Magister Phaedris
            44027, // WANTED: Magister Phaedris
            41818, // WANTED: Majestic Elderhorn
            44292, // WANTED: Majestic Elderhorn
            44015, // WANTED: Mal'Dreth the Corruptor
            44026, // WANTED: Mal'Dreth the Corruptor
            43438, // WANTED: Nameless King
            43625, // WANTED: Nameless King
            43432, // WANTED: Normantis the Deposed
            43612, // WANTED: Normantis the Deposed
            41686, // WANTED: Olokk the Shipbreaker
            44010, // WANTED: Oreth the Vile
            43458, // WANTED: Perrexx
            43630, // WANTED: Perrexx
            42795, // WANTED: Sanaar
            44300, // WANTED: Seersei
            44302, // WANTED: Seersei
            41844, // WANTED: Sekhan
            44294, // WANTED: Sekhan
            44022, // WANTED: Shal'an
            41821, // WANTED: Shara Felbreath
            43619, // WANTED: Shara Felbreath
            44012, // WANTED: Siegemaster Aedrin
            44023, // WANTED: Siegemaster Aedrin
            43456, // WANTED: Skul'vrax
            43631, // WANTED: Skul'vrax
            41838, // WANTED: Slumber
            44293, // WANTED: Slumber
            43429, // WANTED: Syphonus
            43613, // WANTED: Syphonus
            43437, // WANTED: Thane Irglov
            43626, // WANTED: Thane Irglov
            43457, // WANTED: Theryssia
            43632, // WANTED: Theryssia
            43459, // WANTED: Thondrax
            43633, // WANTED: Thondrax
            43450, // WANTED: Tiptog the Lost
            43627, // WANTED: Tiptog the Lost
            43451, // WANTED: Urgev the Flayer
            43628, // WANTED: Urgev the Flayer
            42633, // WANTED: Vorthax
            43614, // WANTED: Vorthax
            43431, // WANTED: Warbringer Mox'na
            42270, // Scourge of the Skies
            44287, // DEADLY: Withered J'im
            43192, // Terror of the Deep
            43448, // The Frozen King
            43193, // Calamitous Intent
            42779, // The Sleeping Corruption
            42269, // The Soultakers
            42819, // Pocket Wizard
            42270, // Scourge of the Skies
            43512, // Ana-Mouz
            44932, // The Nighthold: Ettin Your Foot In The Door
            43985, // A Dark Tide
        };
    }
}