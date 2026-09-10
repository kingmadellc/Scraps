# Naming research — 0.4.8 planning

**Historical shortlist: rejected by the user on September 10, 2026. No name was selected.**

Research date: September 9, 2026 (America/Los_Angeles). Prepared by the independent naming/playtest agent. This is a proposal, not a product rename. No application title, bundle identifier, or save path was changed.

**Editorial recommendation:** test **Rooftop Riffraff**, **Back Alley Legend**, and **Small Paws, Loud City**. The first emphasizes mischievous rooftop exploration; the second emphasizes a neighborhood folk hero; the third expresses a small protagonist with an outsized musical personality. Celebrate the disabled raccoon's agency and character without making disability the joke or the entire identity.

## Evidence status and limits

Completed: public web collision screening and direct US Apple software catalogue queries described below. No exact game/app title collision surfaced for the ten full candidate names in these checks.

**No exact match found does not mean availability or legal clearance.** No trademark database clearance, legal opinion, name reservation, domain/handle availability assessment, or worldwide storefront audit was performed. Search engines can omit titles, and Apple's search API is a ranked search response rather than an exhaustive registry. Removed, unreleased, reserved, localized, or poorly indexed names may be missed.

**No human participant research has been conducted.** There are no participant results, search-volume measurements, conversion measurements, recall measurements, or measured preference percentages. The scores below are subjective editorial judgments, not predicted commercial performance. Nobody was contacted or recruited.

## Weighted editorial rubric

Each dimension is scored from 1 (weak) to 10 (strong). Weighted total out of 100 = discoverability × 3.5 + memorable/pronounceable × 2.5 + concept fit × 2.5 + brand expandability × 1.5. Scores were assigned after the screening, not as an independent experiment. Small score differences should not be treated as meaningful evidence.

- **Discoverability, 35%:** estimated distinctiveness of the complete phrase, spelling clarity, and observed adjacent naming clutter. This is not keyword demand, search rank, or App Store optimization measurement.
- **Memorable/pronounceable, 25%:** editorial assessment of rhythm, brevity, ease of saying and repeating the name. Actual recall remains untested.
- **Concept fit, 25%:** connection to a Seattle raccoon, nighttime treasure hunting, music, and unlikely folk-hero character.
- **Brand expandability, 15%:** ability to support future stories, merchandise, and settings without requiring a different premise. No commercial demand is implied.

## Ranked candidates

All ten are below Apple's 30-character name maximum. “No exact match” below refers only to the bounded checks in this document.

| Rank | Candidate | Discoverability /10 | Memorable /10 | Fit /10 | Expandability /10 | Weighted /100 | Editorial rationale and observed overlap |
| --- | --- | ---: | ---: | ---: | ---: | ---: | --- |
| 1 | Rooftop Riffraff | 9 | 9 | 9 | 8 | 88.5 | Mischievous exploration; “riff” suggests music. No exact game/app match found. Does not explicitly identify the animal without art/subtitle. |
| 2 | Back Alley Legend | 9 | 9 | 8 | 9 | 87.5 | Strongest folk-hero promise. No exact game/app match found. Musical and raccoon elements need art/subtitle. |
| 3 | Small Paws, Loud City | 9 | 8 | 9 | 8 | 86.0 | Characterful small-hero/big-city contrast. No exact game/app match found. Longer and somewhat tagline-like. |
| 4 | Afterhours Bandit | 8 | 9 | 8 | 9 | 84.0 | Nighttime scavenging and masked-raccoon identity. No exact game/app match found. Less musical personality. |
| 5 | Tin Can Renegade | 7 | 8 | 9 | 8 | 79.0 | Scrappy collecting and rebellious attitude. No exact match found, but **Tin Can** is an existing game; retain the complete name if pursued. [Steam listing](https://store.steampowered.com/app/1315980/Tin_Can/) |
| 6 | Rain City Riffraff | 6 | 9 | 9 | 7 | 76.5 | Strong Seattle atmosphere. **Rain City already exists as an animal adventure**, a material adjacent collision despite no exact full-name match. [Steam listing](https://store.steampowered.com/app/1307370/Rain_City/) |
| 7 | Ballard After Dark | 8 | 8 | 7 | 6 | 74.5 | Clear neighborhood and nighttime identity. No exact game/app match found. Could be mistaken for a nightlife guide; constrains expansion beyond Ballard. |
| 8 | Alley Anthem | 6 | 9 | 8 | 7 | 74.0 | Compact and musical. No exact game/app match found; music titles already use it, including YoursTruly's single. [Apple Music](https://music.apple.com/us/album/alley-anthem-feat-jeromyrome-l%C3%BCl%C3%BC-single/1793795423) |
| 9 | Pocket Full of Thunder | 5 | 8 | 9 | 7 | 70.5 | Collecting meets outsized rock-star energy. No exact game/app match found; existing music uses include FateD's single. [Apple Music](https://music.apple.com/us/album/pocket-full-of-thunder-single/1839385739) |
| 10 | Raccoon Rhapsody | 5 | 8 | 8 | 7 | 68.0 | Animal and music are immediately legible. No exact game/app match found; existing song and university database uses add clutter. [Amazon Music](https://music.amazon.in/tracks/B0FSZTPCL2), [York University course material](https://www.eecs.yorku.ca/course_archive/2019-20/F/3421B/EECS3421B_project2.pdf) |

Earlier brainstorm **Tin Can Troubadour** was excluded because it is already Aaron Wilson's artist identity and album name, particularly close to this project's musical positioning. [Artist page](https://www.reverbnation.com/aaronwilson8)

## Exact screening methodology

1. Ran a separate public-web query for each complete candidate in quotation marks followed by `game app`. Also screened the excluded “Tin Can Troubadour.” Reviewed returned results for actual title matches, distinguishing music, incidental words, and unrelated results from game collisions.
2. Ran these three exact-phrase store-domain queries; all returned empty results. Grouped OR queries are only a supplementary screen and are not proof of exhaustive coverage:

```text
site:store.steampowered.com OR site:itch.io "Rain City Riffraff" OR "Small Paws, Loud City" OR "Rooftop Riffraff"
site:store.steampowered.com OR site:itch.io "Tin Can Renegade" OR "Afterhours Bandit" OR "Back Alley Legend"
site:store.steampowered.com OR site:itch.io "Ballard After Dark" OR "Alley Anthem" OR "Pocket Full of Thunder" OR "Raccoon Rhapsody"
```

3. Queried Apple's US software Search API once for each name using Python urllib, with URL-encoded `term`, `entity=software`, `country=us`, and `limit=200`:

```text
https://itunes.apple.com/search?term=<URL-encoded candidate>&entity=software&country=us&limit=200
```

Compared each response's `trackName` after lowercasing and removing all characters except ASCII letters/digits. Checked both exact normalized equality and whether the complete normalized candidate occurred inside a longer returned title. All ten had zero matches by either check. The response counts below are **returned search results, not competing exact names and not search volume**:

| Candidate | Returned results | Exact / candidate-contained title matches |
| --- | ---: | ---: |
| Rooftop Riffraff | 10 | 0 |
| Back Alley Legend | 4 | 0 |
| Small Paws, Loud City | 80 | 0 |
| Afterhours Bandit | 158 | 0 |
| Tin Can Renegade | 3 | 0 |
| Rain City Riffraff | 6 | 0 |
| Ballard After Dark | 1 | 0 |
| Alley Anthem | 12 | 0 |
| Pocket Full of Thunder | 7 | 0 |
| Raccoon Rhapsody | 51 | 0 |

4. Followed up shortened components with official-store searches for “Rain City” and “Tin Can”; both returned the existing games cited above. A “Riff Raff” App Store query returned a character mention in Neff Splash 2.0, not an exact candidate-title collision. [Apple listing](https://apps.apple.com/us/app/neff-splash-2-0/id6444528154)

The live responses were inspected during this session; complete raw response bodies were not archived. This document records the method and observed counts, and repeat searches may differ.

## Official platform and clearance guidance

Apple asks for a distinctive name and accurate metadata, with app names limited to 30 characters. A subtitle can provide additional context; it should not contain irrelevant references or unverifiable claims. [App Review Guidelines, 2.3.7](https://developer.apple.com/app-store/review/guidelines/)

Apple permits subtitles up to 30 characters. A possible neutral subtitle for testing is **Raccoon Treasure Adventure** (26 characters), keeping the animal and activity explicit without packing them into the brand name. This subtitle is a draft and was not collision-screened. [Product-page guidance](https://developer.apple.com/app-store/product-page/)

Apple says search considers text relevance across the title, subtitle, keywords, and primary category, together with behavioral signals such as downloads and reviews. Consequently, a distinctive phrase alone does not establish discoverability or a ranking advantage. [App Store search](https://developer.apple.com/app-store/search/)

USPTO guidance calls for a comprehensive search for similar marks, not merely identical words; likelihood of confusion also depends on related goods or services. Federal trademark searching is one part of clearance, rather than a guarantee of registrability. The positive Steam and music findings should therefore inform further review even when the full proposed name differs. [Likelihood of confusion](https://www.uspto.gov/trademarks/search/likelihood-confusion), [Comprehensive clearance searches](https://www.uspto.gov/trademarks/search/comprehensive-clearance-search-similar-trademarks)

## Proposed blinded participant test — not conducted

Test only the top three initially: Rooftop Riffraff, Back Alley Legend, Small Paws, Loud City. Do not recruit or contact anyone until separately authorized. The protocol is a plan, and contains no invented participant evidence.

1. Recruit 12 consenting adult prospective players for a small qualitative study, including both Seattle-familiar and Seattle-unfamiliar people and people with varied mobile-game experience. Seek disabled participants' perspectives without asking for diagnoses or making anyone represent all disabled players. Offer accessible response formats and compensation if recruitment is approved. Twelve people would support directional observations, not population-level conclusions.
2. Prepare identical plain-text name cards in the same font, size, color, and layout. Hide the team's preferred name, scores, collision findings, project history, and existing Jimothy name. Assign neutral codes A/B/C in the data sheet. The moderator should not be told the preferred candidate where practical; participants necessarily see the tested words.
3. Balance the six possible presentation orders, two participants per order. Before revealing the game concept, show each name briefly. Ask what kind of game they expect, how they pronounce it, and how they would spell it when telling a friend. Record verbatim responses rather than interpreting hesitation as dislike.
4. Use a short unrelated task, then ask for unaided recall. Record exact recall, partial recall, and confusions separately. Do not claim delayed or long-term recall from this brief test.
5. Show the same concise concept description and identical unlabeled gameplay clip to everyone: a disabled raccoon exploring Seattle at night, collecting treasures, evading observers, and building a den, with a rock-and-roll folk-hero personality. Then ask each name's fit, misleading implications, and first/second/third preference. Ask whether any wording feels demeaning or promises gameplay the clip does not show.
6. Only after the primary responses, show identical app-card mockups with the same artwork and draft subtitle. Ask whether text is readable at phone size and whether participants can tell that this is a raccoon exploration game. Do not vary artwork or subtitle between candidates.
7. Report anonymized response counts, actual quotations with consent, expectation mismatches, and order effects. Keep participant preference, recall, accessibility feedback, and the editorial rubric separate. Do not convert a small convenience sample into market-share, conversion, or search-volume estimates. If the leading names are close, report uncertainty rather than manufacture a winner.

A permanent rename should follow this evidence review and appropriate clearance. Existing product/save identifiers remain unchanged in the meantime.
