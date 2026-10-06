---
name: Vitalis Blood Community
colors:
  surface: '#f4faff'
  surface-dim: '#cfdce4'
  surface-bright: '#f4faff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#e9f6fd'
  surface-container: '#e3f0f8'
  surface-container-high: '#ddeaf2'
  surface-container-highest: '#d7e4ec'
  on-surface: '#111d23'
  on-surface-variant: '#5a4040'
  inverse-surface: '#263238'
  inverse-on-surface: '#e6f3fb'
  outline: '#8e706f'
  outline-variant: '#e2bebd'
  surface-tint: '#b81c34'
  primary: '#a30428'
  on-primary: '#ffffff'
  primary-container: '#c6283d'
  on-primary-container: '#ffe1e1'
  inverse-primary: '#ffb3b3'
  secondary: '#685b5e'
  on-secondary: '#ffffff'
  secondary-container: '#f1dee2'
  on-secondary-container: '#6f6164'
  tertiary: '#8d3033'
  on-tertiary: '#ffffff'
  tertiary-container: '#ac4749'
  on-tertiary-container: '#ffe1df'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffdad9'
  primary-fixed-dim: '#ffb3b3'
  on-primary-fixed: '#400009'
  on-primary-fixed-variant: '#920022'
  secondary-fixed: '#f1dee2'
  secondary-fixed-dim: '#d4c2c6'
  on-secondary-fixed: '#23191c'
  on-secondary-fixed-variant: '#504447'
  tertiary-fixed: '#ffdad8'
  tertiary-fixed-dim: '#ffb3b1'
  on-tertiary-fixed: '#410007'
  on-tertiary-fixed-variant: '#81272b'
  background: '#f4faff'
  on-background: '#111d23'
  surface-variant: '#d7e4ec'
typography:
  display-hero:
    fontFamily: Plus Jakarta Sans
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  display-hero-mobile:
    fontFamily: Plus Jakarta Sans
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.015em
  headline-lg-mobile:
    fontFamily: Plus Jakarta Sans
    fontSize: 24px
    fontWeight: '700'
    lineHeight: 32px
    letterSpacing: -0.01em
  headline-md:
    fontFamily: Plus Jakarta Sans
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
    letterSpacing: -0.01em
  headline-sm:
    fontFamily: Plus Jakarta Sans
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  title-md:
    fontFamily: Plus Jakarta Sans
    fontSize: 16px
    fontWeight: '600'
    lineHeight: 24px
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 15px
    fontWeight: '400'
    lineHeight: 24px
  body-sm:
    fontFamily: Inter
    fontSize: 13px
    fontWeight: '400'
    lineHeight: 20px
  label-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 15px
    fontWeight: '600'
    lineHeight: 20px
  label-md:
    fontFamily: Plus Jakarta Sans
    fontSize: 13px
    fontWeight: '600'
    lineHeight: 18px
  label-sm:
    fontFamily: Plus Jakarta Sans
    fontSize: 11px
    fontWeight: '700'
    lineHeight: 16px
    letterSpacing: 0.04em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  gutter: 1.5rem
  gutter-mobile: 1rem
  margin: 2rem
  margin-mobile: 1rem
  space-xs: 0.25rem
  space-sm: 0.5rem
  space-md: 1rem
  space-lg: 1.5rem
  space-xl: 2.5rem
---

## Brand & Style

This design system serves a humanitarian blood donation community and appointment scheduling platform. The core philosophy centers on warmth, clinical safety, community empathy, and effortless civic engagement. It deliberately rejects the cold, sterile, intimidating atmosphere of legacy hospital interfaces as well as the sensationalized, alarming aesthetics often associated with urgent medical appeals.

The visual direction follows a **Soft Modern Civic** aesthetic:
- **Calm, Humanized Vitality:** Red is deployed strictly as an intentional signal of life and action, framed by generous fields of clean white, soft slate foundations, and pale blush surfaces that neutralize visual anxiety.
- **Approachable Clinical Trust:** High-legibility typography, soft boundary treatments, and generous spatial margins evoke hospitality, clear communication, and modern civic pride.
- **Empowering Community Spirit:** Gamified milestones (donation history, community blood banks, life-counter badges) are treated with warm optimism rather than clinical bureaucracy.

## Colors

The palette establishes an emotional balance between urgency and comfort.

### Hierarchy & Application Rules
- **Primary Red (`#C6283D`) & Crimson (`#D93A52`):** Reserved exclusively for hero calls-to-action, active progress indicators, vital donation metrics, and life-impact highlights. Red is never applied to massive full-bleed backgrounds.
- **Secondary Surfaces (`#FBE8EC`, `#FFF4F5`):** Soft rose and warm blush create subtle contrast for card hero states, highlighted donation tips, appointment alert banners, and active navigation pills without harsh contrast.
- **Accent Tones (`#E87575`, `#F3A683`):** Warm coral and soft orange support progress steppers, celebratory badges, donor tier achievements, and informative data visualizations.
- **Neutrals:**
  - Canvas Background: `#F8FAFC` provides a clean, glare-free foundation.
  - Surface Pure: `#FFFFFF` for elevated modular cards and input surfaces.
  - Divider & Border: `#E2E8F0` keeps sections distinct without visual clutter.
  - Structural Gray: `#EEF2F5` for input container fills, table alternating stripes, and inactive tracks.
  - Text Primary: `#263238` delivers high contrast without the harshness of pure black.
  - Text Secondary: `#64748B` balances subheadings, timestamps, and secondary metadata.
- **System Feedback:**
  - Success Green: `#2E9D67` (Eligibility confirmed, slot booked, screening passed)
  - Warning Amber: `#E5A62A` (Expiring reserve, preparation reminders, fasting checklist)
  - Error Red: `#DC3545` (Form validation, disqualification alert, critical supply shortage)
  - Info Blue: `#2563EB` (Medical guidelines, logistics details, donor FAQs)

## Typography

The type system blends the energetic, rounded warmth of **Plus Jakarta Sans** for structural headers and actions with the neutral, systematic clarity of **Inter** for dense health intake questionnaires, donor guidance notes, and verification sheets.

### Typographic Principles
- **Generous Leading:** Health guidelines, screening questions, and pre-donation criteria always employ relaxed line-heights (`1.55` to `1.6`) to prevent cognitive exhaustion.
- **Controlled Numbers & Tabular Alignment:** Donor counts, blood units (ml), date stamps, and identification values must utilize tabular lining numbers to preserve balance across registration tables and donor dashboards.
- **Heading Weighting:** Headlines lean toward `600` (SemiBold) and `700` (Bold) to project safety, clarity, and dependable institutional strength.

## Layout & Spacing

The layout operates on a standard **12-column responsive fluid grid** for desktop and tablet viewports, collapsing into a **4-column structured stack** on mobile devices.

### Viewport Behaviors
- **Desktop (1200px+):** 12 columns, `1.5rem` (24px) gutters, and up to a max content container of `1240px` centered within a `#F8FAFC` foundation. Donor registration workflows feature a comfortable 7:5 ratio split (form inputs on the primary side, appointment live-summary card on the sticky secondary rail).
- **Tablet (768px – 1199px):** 8 columns, `1.5rem` (24px) gutters, `1.5rem` margins. Side panels collapse under primary registration blocks.
- **Mobile (< 768px):** 4 columns, `1rem` (16px) gutters, `1rem` (16px) side margins. Sticky bottom bar for vital actions (e.g., "Xác nhận Đăng ký Hiến máu").

### Spacing Cadence
Use consistent vertical increments:
- `space-xs` (4px) for badge inner vertical padding and icon-to-label separation.
- `space-sm` (8px) between form labels and input boxes, or between paired action buttons.
- `space-md` (16px) for inner card content sections and dense list rows.
- `space-lg` (24px) for card interior padding and step-by-step intake clusters.
- `space-xl` (40px) for layout section divisions and structural headline gaps.

## Elevation & Depth

This design system avoids heavy shadows, dark drop offsets, or flat brutalist lines. It employs an **Atmospheric Tinted Elevation** model where subtle shadows borrow a faint crimson hue to create organic, life-infused warmth without looking blurred or dirty.

### Elevation Hierarchy
- **Level 0 (Flat / Canvas):** Applied to the main page background (`#F8FAFC`) and nested table headers.
- **Level 1 (Resting Cards & Surfaces):** Default state for cards, forms, and donor metric panels.
  - `box-shadow: 0 4px 20px -2px rgba(198, 40, 61, 0.05), 0 2px 6px -1px rgba(0, 0, 0, 0.04);`
  - Border: `1px solid #E2E8F0`
  - Surface: `#FFFFFF`
- **Level 2 (Hover & Active Interactive Cards):** Applied when donors hover blood drive location cards or date selector tiles.
  - `box-shadow: 0 10px 25px -3px rgba(198, 40, 61, 0.08), 0 4px 10px -2px rgba(0, 0, 0, 0.04);`
  - Border: `1px solid #FBE8EC`
  - Transition: `transform 180ms ease, box-shadow 180ms ease` (translate -2px)
- **Level 3 (Modals, Toast Alerts, Sticky Actions):** Applied to floating booking summaries and medical consent dialogues.
  - `box-shadow: 0 20px 35px -5px rgba(38, 50, 56, 0.12), 0 8px 16px -4px rgba(198, 40, 61, 0.06);`
  - Border: `1px solid rgba(226, 232, 240, 0.8)`
- **Backdrop Dimming:** Modals use `#263238` at `35%` opacity with a subtle `backdrop-filter: blur(4px)`.

## Shapes

The design system maintains a tactile, approachable form factor with deliberate corner radii designed to feel friendly, reassuring, and ergonomic:

- **Cards & Data Modules:** Strict `18px` border radius (`rounded-lg` equivalent in this system). This softens the healthcare experience and avoids clinical rigidity.
- **Interactive Controls (Buttons, Inputs, Dropdowns):** Uniform `12px` border radius (`rounded-md`). Provides comfortable click/tap targets with a modern geometric curve.
- **Pill Containers (Badges, Status Chips, Step Nodes):** Full pill (`9999px`) roundedness to distinctly separate categorizations, blood type badges (e.g., "O-", "A+"), and operational statuses from standard rectangular inputs.

## Components

### Buttons
- **Primary Action (Call-to-Donate):** Background `#C6283D`, text `#FFFFFF`, radius `12px`, padding `12px 24px`, font `Plus Jakarta Sans 600`. Hover state shifts to `#D93A52` with elevation level 2. Active state uses `#A71D2F`.
- **Secondary Action:** Background `#FFF4F5`, border `1px solid #FBE8EC`, text `#C6283D`. Hover transitions to `#FBE8EC`.
- **Ghost / Neutral:** Background transparent, text `#263238`, hover background `#EEF2F5`.

### Input Fields & Selectors
- **Container:** Height `48px`, background `#FFFFFF`, border `1px solid #E2E8F0`, radius `12px`, padding `0 16px`, font `Inter 15px`.
- **Focus State:** Border shifts to `#C6283D`, accompanying an outer glow ring of `0 0 0 3px rgba(198, 40, 61, 0.12)`.
- **Form Labels:** `Plus Jakarta Sans 600`, size `13px`, text `#263238`, `margin-bottom: 6px`. Required asterisks rendered in `#C6283D`.

### Checkboxes & Radios (Health Screening Declarations)
- **Checkboxes:** `20px x 20px`, radius `6px`, border `1.5px solid #CBD5E1`. Checked state fills `#C6283D` with a crisp white tick mark.
- **Radio Buttons:** `20px x 20px`, radius `50%`. Selected state features an active inner `#C6283D` dot with `3px` white clearance.
- **Questionnaire Card Rows:** Tapping anywhere on a health checklist row toggles the item, applying a subtle `#FFF4F5` tint to the row container.

### Chips & Blood Group Badges
- **Blood Group Pill:** Height `28px`, padding `0 12px`, background `#FFF4F5`, border `1px solid #FBE8EC`, text `#C6283D`, font `Plus Jakarta Sans 700 13px`.
- **Status Badges:** Full pill with an interior `6px` status dot.
  - Active / Open: Background `#ECFDF5`, text `#2E9D67`.
  - Pending Review: Background `#FFFBEB`, text `#E5A62A`.
  - Urgent Need: Background `#FEF2F2`, text `#DC3545`.

### Cards & Panels
- **Structure:** Background `#FFFFFF`, border `1px solid #E2E8F0`, radius `18px`, padding `24px`, elevation level 1.
- **Drive Location Card:** Includes time badge, capacity progress bar, host hospital label, and an immediate "Đăng ký" primary action button.

### Specialized Community Components
- **Donation Progress Stepper:** Visual breadcrumb spanning `[Thông tin cá nhân] -> [Khảo sát sức khỏe] -> [Chọn địa điểm & giờ] -> [Mã QR Check-in]`. Active node uses `#C6283D` with a soft pulsing shadow; completed nodes turn `#2E9D67`.
- **Blood Urgency Metric Meter:** Horizontal micro-bar displaying regional inventory levels (Khẩn cấp / Ổn định) with clean dual-tone fills.
- **Digital Donor Card (Thẻ Hiến Máu Điện Tử):** Radius `18px`, deep gradient `#C6283D` to `#9B1829`, high-contrast white typography, holographic-style foil chip accent, containing donor name, blood group, lifetime donations count, and an embedded QR code for fast intake check-in.