# Environment 0.4.4

Ten storefront interiors now communicate their trade: marine rope and floats, copper cookware and herbs, bakery trays and scored loaves, pub taps, fish on ice, copper tankards, billiard cues, botanical displays, espresso equipment and coffee bags, and vinyl sleeves and records. Frontages vary through tiled dados, timber panels, transom spacing, striped fabric awnings, standing-seam metal canopies and compact cast hoods. Original fictional shop identities remain.

New planting includes sword ferns with curved rachises, hostas, hydrangeas, trailing vines, supported window boxes and suspended baskets. Existing park beds receive mixed planting; cedar trunks taper and end within foliage. Leaf relief scales to leaf size, with subtle midrib/vein shading. New decorative plants avoid the street navigation lanes.

Rooftops have ten activity identities: yoga, pizza kitchen and bar with sink, shared work/lunch table, record listening, painting, edible gardening, bistro dining, chess, lookout field notes, and coffee loungers. Activity zones have inset thin timber decking, pots and warmer practical lights; four terraces have supported festoons. Furniture has matching colliders, including the underside of round tabletops. Existing crossing and find lanes are reserved.

Rainier uses an authored asymmetric summit profile and an indexed relief mesh, with glacial channels, dark rock cleavers, blue ice and snow that stays legible through distance haze. It is a stylized scenic composition, not a geographically surveyed reproduction. A single combined floatplane mesh has a high wing, twin rounded floats and struts. First flight begins after18seconds of active play; the38second crossing repeats every142seconds. Timing pauses in menus and Stash. The initial offscreen portion means the aircraft enters the avenue view later in the pass.

Actual Unity captures: PlaytestCaptures/environment-v044,1600×900 at production2×MSAA. Frame00 uses the gameplay camera; remaining frames are directed inspection cameras. Frame16 stages the plane along its real path to inspect silhouette; it is not proof of natural flight timing. Native/Web compiled checks are recorded separately below.

Reference: Ballard window depth, painted frames and overflowing greenery were studied from https://onthegrid.city/seattle/ballard/palm-room and historic storefront photos from https://voicemap.me/tour/seattle/historic-ballard-the-quirky-scandinavian-enclave-just-north-of-downtown-seattle/sites/cors-wegener-building-5000-20th-ave-nw . No downloaded brand artwork is used. Rainier references and mesh counts are in RAINIER-SEAPLANE.md.

Limits: the scene remains a stylized prototype and does not yet match the original concept render's realism. Physical iPhone/iPad performance has not been profiled for this build. The underlying character and gait are unchanged in this environment pass.

Validation: environment-access audit passed all63 supported search sites and18reachable rooftop approaches across6roof grids; actual-motor rooftop crossings and district NPC navigation audits passed; EditMode19/19passed. Native0.4.4built and passed strictdeepcodesign. Isolated native smoke verified forward movement and Den travel, no shader/NaN/runtime exceptions, saving suppressed. Natural plane crossing is visible in AgentPlaytests/v044-environment/frame-1.png at00:47, without changing flight state.

Web build passed (117,183,827bytes uncompressed). Isolated desktop WebKit touch-context smoke confirmed runtime0.4.4, new-game entry, forward movement and terrain/material rendering with no console/page errors; screenshots and states are in PlaytestCaptures/web-v044-environment. This is not a physical iPad benchmark. Normal Mac0.4.4 menu was reopened and visually verified.
